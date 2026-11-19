using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;
using System.Diagnostics;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Implementation of the duplicate resolver
    /// </summary>
    public class DuplicateResolver : IDuplicateResolver
    {

        private string errMsg = string.Empty;
        private int errCode = 0;

        //private 

        #region IDuplicateResolver Members

        public StrokeData Resolve(StrokeData data, UploadLog log)
        {
            errMsg = string.Empty;
            errCode = 0;

            // get key fields
            var facility = log.Facility;

            facility.Format.KeyEurekaField.EnsureLoad();
            var keyFields = facility.Format.KeyEurekaField.ToDictionary(
                ef => ef.Name, 
                ef => (object)(from v in data.StrokeFieldValue
                               where ef.ID == v.EurekaField.ID
                               select v.RawValue).FirstOrDefault());
            //keyFields["admissionDate"] = data.AdmissionDate;
            //keyFields["patientID"] = data.PatientID;
            //var admissionDate = data.AdmissionDate;
            //var patientID = data.PatientID;


            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // implement temporal overlap feature
            TemporalOverlapResult = TemporalOverlapManager.Verify(facility, data, out errMsg);
            if (TemporalOverlapResult != EnumTemporalOverlapResult.CheckKeyFields)
            {
                if (TemporalOverlapResult == EnumTemporalOverlapResult.Ignore)
                {
                    // remove source values from object context to avoid them being added to db
                    RemoveFromContext(data, ctx);
                    errCode = -1;
                }
                return data;
            }
            
            // check key fields
            //StrokeData existing = DataFacade.GetStrokeData(ctx, facility, patientID, admissionDate);
            StrokeData existing = DataFacade.GetStrokeData(ctx, facility, keyFields);

            // counter of changes found and applied
            int changesAppliedCount = 0;
            // counter of changes found 
            int correctionsFound = 0;

            if (existing == null)
                return data;

            // TODO: otherwise merge new data with existing
            ILookup<int, StrokeFieldValue> existingMap = existing.StrokeFieldValue
            .ToLookup(v => v.EurekaField.ID, v => v);

            // select all values
            IEnumerable<StrokeFieldValue> newVals = data.StrokeFieldValue; // .Where(v =>(v.Status ?? 0) == 0);
            ILookup<int, StrokeFieldValue> newMap = newVals.ToLookup(v => v.EurekaField.ID, v => v);

            // TODO: add audit log
            // TODO: check error status of the source value

            // delegate to add a new value into existing stroke data record
            Action<StrokeData, StrokeFieldValue, EnumActionType> addNewValue = (d, v,type) =>
                {
                    if (v != null && (v.Status ?? 0) == 0)
                    {
                        StrokeFieldValue val = v.Clone(ctx,type);
                        d.StrokeFieldValue.Add(val);
                        // count changes and corrections
                        if (type == EnumActionType.Update)
                        {
                            correctionsFound++;
                            if ((v.Status ?? 0) == 0) changesAppliedCount++;
                        }
                    }
                };

            // delegate to update a new value into existing stroke data record
            Action<StrokeFieldValue, StrokeFieldValue> updateNewValue = (vNew, vOld) =>
            {
                if (vNew != null && vOld != null)
                {
                    if (!vNew.Equals(vOld))
                    {
                        if ((vNew.Status ?? 0) == 0)
                        {
                            vNew.CopyTo(ctx, vOld);
                            vOld.LastModifiedDate = DateTime.Now;
                            vOld.LastModifiedBy = (int?)DataFacade.GetCurrentUser();
                            changesAppliedCount++; // count corrections
                        }
                        else // error encountered during correction
                        {
                            vNew.CopyAudit(ctx, vOld, null);
                        }
                        correctionsFound++; // count changes
                        
                    }
                    else if ((vNew.Status ?? 0) == 0) // no change encountered
                    {
                        //vNew.CopyAudit(ctx, vOld,null);
                        vNew.RemoveAudit(ctx);
                    }
                    else // error encountered during correction 
                    {
                        vNew.CopyAudit(ctx, vOld, null);
                    }
                    
                }
            };

            // delegate to delete existing stroke value
            Action<StrokeData, StrokeFieldValue> deleteValue = (d, v) =>
            {
                if (v != null && (v.Status ?? 0) == 0)
                {
                    d.StrokeFieldValue.Remove(v);
                    ctx.DeleteObject(v);
                }
            };


            foreach (IGrouping<int, StrokeFieldValue> group in newMap)
            {
                List<StrokeFieldValue> listNew = group.ToList();
                int fieldID = group.Key;

                // check if corresponding field is present in the existing stroke data record
                if (existingMap.Contains(fieldID))
                {

                    List<StrokeFieldValue> listExisting = existingMap[fieldID].ToList();
                    int cntNew = 0;
                    if (listNew != null)
                        cntNew = listNew.Count;
                    int cntExisting = 0;

                    if (listExisting != null)
                        cntExisting = listExisting.Count;

                    if (cntNew == 1 && cntExisting == 0)
                    {
                        StrokeFieldValue valNew = listNew[0];
                        addNewValue(existing, valNew,EnumActionType.Create);

                    }
                    else if (cntNew == 1 && cntExisting == 1)
                    {
                        updateNewValue(listNew[0], listExisting[0]);
                    }
                    else if (cntNew == 0 && cntExisting >= 1)
                    {
                        // do nothing
                    }
                    else if (cntNew > 1 && cntExisting == 0)
                    {
                        listNew.ForEach(v =>
                        {
                            addNewValue(existing, v, EnumActionType.Create);
                        });
                    }
                    else if (cntNew > 1 && cntExisting > 1)
                    {
                        // remove existing values
                        listExisting.ForEach(v =>
                        {
                            deleteValue(existing, v);
                        });

                        listNew.ForEach(v =>
                        {
                            addNewValue(existing, v, EnumActionType.Update);
                        });
                    }
                }



            }


            // remove source values from object context to avoid them being added to db
            RemoveFromContext(data, ctx);

            // store changes count and corrections applied
            if (log.ChangesCount == null) log.ChangesCount = 0;
            if (log.CorrectionsCount == null) log.CorrectionsCount = 0;

            // count per fields (not per record)
            //log.ChangesCount += (changesAppliedCount>0?1:0);
            //log.CorrectionsCount += (correctionsFound>0?1:0);
            log.ChangesCount += changesAppliedCount;
            log.CorrectionsCount += correctionsFound;
            return existing;
        }

        /// <summary>
        /// remove source values from object context to avoid them being added to db
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ctx"></param>
        private void RemoveFromContext(StrokeData data, EurekaStrokeEntities ctx)
        {
            // remove source values from object context to avoid them being added to db
            var fields = data.StrokeFieldValue.ToList();
            foreach (var v in fields)
            {
                ctx.Detach(v);
            }
            ctx.Detach(data);

        }

        #endregion

        #region IError Members

        public int ErrorCode
        {
            get { return errCode; }
        }

        public string ErrorMessage
        {
            get { return errMsg; }
        }

        public EnumTemporalOverlapResult TemporalOverlapResult { get; private set; }

        #endregion
    }
}
