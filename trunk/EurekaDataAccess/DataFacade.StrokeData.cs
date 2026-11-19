using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Eureka.Stroke;

namespace Eureka.Stroke.Data
{
    public static partial class DataFacade
    {
        /*
        /// <summary>
        /// Get UploadLog records
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static StrokeData GetStrokeData(EurekaStrokeEntities ctx, Facility facility, string patientID, DateTime admissionDate)
        {
            var item = (from i in ctx.StrokeData.Include("StrokeFieldValues")
                        from l in i.UploadLog
                        where i.PatientID == patientID
                        && i.AdmissionDate == admissionDate
                        && l.Facility.ID == facility.ID
                    orderby i.CreatedDate descending
                    select i).FirstOrDefault();

            if (item != null)
            {
                item.StrokeFieldValue.EnsureLoad();
                //item.UploadLogs.EnsureLoad();
            }

            return item;
        }
        */

        public static StrokeData GetStrokeData(EurekaStrokeEntities ctx, Facility facility, IDictionary<string, object> keyFields)
        {
            // look for all StrokeData records
            var query2 = (from i in ctx.StrokeData.Include("StrokeFieldValue")
                        orderby i.CreatedDate descending
                        select i);

            // but filter them by key fields
            foreach (var k in keyFields)
            {
                var pair = k;
                var value = (string) pair.Value;
                query2 = query2.Where(
                    i => i.StrokeFieldValue.Any(
                        v => v.EurekaField.Name == pair.Key && v.RawValue == value
                    )
                ).OrderByDescending(i => i.CreatedDate);
            }

            // and filter them by specified facility
            var query = (from i in query2
                         from l in i.UploadLog
                         where l.Facility.ID == facility.ID
                         select i);

            /*
            // pure test implementation
            var patientID = ((string) keyFields["PatientID"]);
            var query = (from i in ctx.StrokeData.Include("StrokeFieldValue")
                         where i.PatientID == patientID
                            //&& i.AdmissionDate == (DateTime)keyFields["AdmissionDate"]
                         from l in i.UploadLog
                         where
                            l.Facility.ID == facility.ID
                         orderby i.CreatedDate descending
                         select i);
            */

            try
            {
                var item = query.FirstOrDefault();

                if (item != null)
                {
                    item.StrokeFieldValue.EnsureLoad();
                    //item.UploadLogs.EnsureLoad();
                }

                return item;
            }
            catch (Exception ex)
            {
                // TODO write to log about this exception
                return null;
            }

        }

        public static StrokePivotData GetPivotStrokeData()
        {
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            StrokePivotData data = ctx.GetPivotStrokeData();
            return data;
        }

        public static StrokePivotData GetPivotFacilityData(int facilityID)
        {
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            StrokePivotData data = ctx.GetPivotFacilityData(facilityID);
            return data;
        }

        public static DataSet GetPivotFacilitiesData(string facilitiesIDs)
        {
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var data = ctx.GetPivotFacilitiesData(facilitiesIDs);
            return data;
        }

        public static DataSet GetPivotReportData(int userID, string reportingSource)
        {
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var data = ctx.GetPivotReportData(userID, reportingSource);
            return data;
        }

        
    }
}

