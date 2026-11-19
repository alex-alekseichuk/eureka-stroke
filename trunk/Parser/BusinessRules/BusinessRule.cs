using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;

namespace Eureka.Stroke.Parser
{

    /// <summary>
    /// Executes a set of business rules
    /// </summary>
    public static class BusinessRuleValidator
    {
        //The time-sequence rules are as follows (alien)
        //1. if gs_ctcompdatetime_precision = 5 and jc_arrdatetime_precision = 5, then gs_ctcompdatetime must come after jc_arrdatetime
        //2. if gs_discdatetime_precision = 5 and jc_admitdate_precision = 5, then gs_discdatetime must come after jc_admitdate
        //3. if jc_ivthrodttm_precision = 5 and gs_ctcompdatetime_precision = 5, then jc_ivthrodttm must come after gs_ctcompdatetime
        //4. if jc_ivthrodttm_precision = 5 and gs_lastknownwell_precision = 5, then jc_ivthrodttm must come after gs_lastknownwell
        //5. if gs_iatpadatetime_precision = 5 and gs_ctcompdatetime_precision = 5, then gs_iatpadatetime must come after gs_ctcompdatetime
        //6. if gs_iatpadatetime_precision = 5 and gs_lastknownwell_precision = 5, then gs_iatpadatetime must come after gs_lastknownwell

        // Eureka
        // 1. GS.ctcompdatetime.Precision=5 JC.Arrdatetime.Precision=5 GS.Ctcompdatetime > JC.Arrdatetime 
        // 2. GS.Discdatetime.Precision JC.Admitdate.Precision GS.Discdatetime > AdmissionDate
        // 3. JC.Ivthrodttm.Precision=5 GS.ctcompdatetime.Precision=5 JC.Ivthrodttm > GS.Ctcompdatetime
        // 4. JC.Ivthrodttm.Precision=5 GS.Lastknownwell.Precision=5 JC.Ivthrodttm > GS.Lastknownwell
        // 5. GS.iatpadatetime.Precision=5 GS.ctcompdatetime.Precision=5 GS.Iatpadatetime > GS.Ctcompdatetime
        // 6. GS.iatpadatetime.Precision=5 GS.Lastknownwell.Precision=5 GS.Iatpadatetime > GS.Lastknownwell

        static BusinessRule[] rules = null;

        static BusinessRuleValidator()
        {
            BusinessRule rule1 = new BusinessRule()
            {
                RuleID = 1,
                FieldNames = new[] { "GS.ctcompdatetime.Precision", "JC.Arrdatetime.Precision", "GS.Ctcompdatetime", "JC.Arrdatetime" },
                RuleDescription = "if GS.ctcompdatetime.Precision=5 && JC.Arrdatetime.Precision=5 then GS.Ctcompdatetime > JC.Arrdatetime ",
                RuleValues = "GS.ctcompdatetime.Precision={0} JC.Arrdatetime.Precision={1} GS.Ctcompdatetime={2} JC.Arrdatetime={3} "
            };

            BusinessRule rule2 = new BusinessRule()
            {
                RuleID = 2,
                FieldNames = new[] { "GS.Discdatetime.Precision", "JC.Admitdate.Precision", "GS.Discdatetime", "AdmissionDate" },
                RuleDescription = "if GS.Discdatetime.Precision=5 && JC.Admitdate.Precision=5 then GS.Discdatetime > AdmissionDate ",
                RuleValues = "GS.Discdatetime.Precision={0} JC.Admitdate.Precision={1} GS.Discdatetime={2} AdmissionDate={3} "
            };

            BusinessRule rule3 = new BusinessRule()
            {
                RuleID = 3,
                FieldNames = new[] { "JC.Ivthrodttm.Precision", "GS.ctcompdatetime.Precision", "JC.Ivthrodttm", "GS.Ctcompdatetime" },
                RuleDescription = "if JC.Ivthrodttm.Precision=5 && GS.ctcompdatetime.Precision=5 then JC.Ivthrodttm > GS.Ctcompdatetime ",
                RuleValues = "JC.Ivthrodttm.Precision={0} GS.ctcompdatetime.Precision={1} JC.Ivthrodttm={2} GS.Ctcompdatetime={3} "
            };

            BusinessRule rule4 = new BusinessRule()
            {
                RuleID = 4,
                FieldNames = new[] { "JC.Ivthrodttm.Precision", "GS.Lastknownwell.Precision", "JC.Ivthrodttm", "GS.Lastknownwell" },
                RuleDescription = "if JC.Ivthrodttm.Precision=5 && GS.Lastknownwell.Precision=5 then JC.Ivthrodttm > GS.Lastknownwell ",
                RuleValues = "JC.Ivthrodttm.Precision={0} GS.Lastknownwell.Precision={1} JC.Ivthrodttm={2} GS.Lastknownwell={3} "
            };

            BusinessRule rule5 = new BusinessRule()
            {
                RuleID = 5,
                FieldNames = new[] { "GS.iatpadatetime.Precision", "GS.ctcompdatetime.Precision", "GS.Iatpadatetime", "GS.Ctcompdatetime" },
                RuleDescription = "if GS.iatpadatetime.Precision=5 && GS.ctcompdatetime.Precision=5 then GS.Iatpadatetime > GS.Ctcompdatetime ",
                RuleValues = "GS.iatpadatetime.Precision={0} GS.ctcompdatetime.Precision={1} GS.Iatpadatetime={2} GS.Ctcompdatetime={3} "
            };

            BusinessRule rule6 = new BusinessRule()
            {
                RuleID = 6,
                FieldNames = new[] { "GS.iatpadatetime.Precision", "GS.Lastknownwell.Precision", "GS.Iatpadatetime", "GS.Lastknownwell" },
                RuleDescription = "if GS.iatpadatetime.Precision=5 && GS.Lastknownwell.Precision=5 then GS.Iatpadatetime > GS.Lastknownwell ",
                RuleValues = "GS.iatpadatetime.Precision={0} GS.Lastknownwell.Precision={1} GS.Iatpadatetime={2} GS.Lastknownwell={3} "
            };

            rules = new[] { rule1, rule2, rule3, rule4, rule5, rule6 };
            //rules = new[] { rule6 };


        }

        public static Result Validate(StrokeData data, UploadLog log, int rowNumber)
        {
            Result res = Result.Done;
            
            rules.ForEach((r) => {
                BusinessRuleCheckResults resEntity;
                var resRule = r.Validate(data, log, rowNumber, out resEntity);
                if (resRule == Result.Error)
                    res = resRule;
            }
            );

            return res;
        }
    }

    /// <summary>
    /// Business rule item
    /// </summary>
    public class BusinessRule
    {
        public int RuleID { get; set; }
        public string[] FieldNames { get; set; }
        public string RuleDescription { get; set; }
        public string RuleValues { get; set; }

        /// <summary>
        /// Validate business rule instance
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <param name="resEntity"></param>
        /// <returns></returns>
        public Result Validate(StrokeData data, UploadLog log, int rowNumber, out BusinessRuleCheckResults resEntity)
        {
            Result res = Result.Done;
            bool yes = true;
            resEntity = null;
            string errMsg = "";
            if (data != null)
            {
                int? precision1 = DataFacade.GetStrokeFieldValueInt(data, FieldNames[0]);
                int? precision2 = DataFacade.GetStrokeFieldValueInt(data, FieldNames[1]);
                DateTime? date1 = DataFacade.GetStrokeFieldValueDate(data, FieldNames[2]);
                DateTime? date2 = DataFacade.GetStrokeFieldValueDate(data, FieldNames[3]);

                if (precision1.HasValue && precision2.HasValue)
                {
                    if (date1.HasValue && date2.HasValue)
                    {
                        if (precision1.Value == 5 && precision2.Value == 5)
                        {
                            yes = date1.Value > date2.Value;
                        }
                    }
                }

                if (!yes)
                {
                    errMsg = string.Format("Rule {0} failed: ", RuleID) + string.Format(RuleValues, 
                        precision1 != null ? precision1.Value.ToString() : "null",
                        precision2 != null ? precision2.Value.ToString() : "null",
                        date1 != null ? date1.Value.ToString() : "null",
                        date2 != null ? date2.Value.ToString() : "null"
                        );
                }
            }

            res = yes ? Result.Done : Result.Error;
            if (!yes)
            {
                resEntity = BusinessRuleCheckResults.CreateBusinessRuleCheckResults(0, RuleID, true, errMsg);
                data.Status = (short)res;
                resEntity.StrokeData = data;
                if (log.ErrorCount == null) log.ErrorCount = 0;
                log.ErrorCount++;
                var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
                AuditLog logAudit = DataFacade.CreateAuditLog(ctx, (int)res,
                        false,
                        log,
                         FieldNames[2] + "," + FieldNames[3],
                         errMsg, "", rowNumber);
            }

            return res;
        }

    }


}
