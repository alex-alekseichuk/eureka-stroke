using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Eureka.Stroke;

namespace WebApplication
{

    /// <summary>
    /// Summary description for ReportHelper
    /// </summary>
    public class ReportHelper
    {
        public ReportHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Get error report data
        /// </summary>
        /// <param name="uploadLogID"></param>
        /// <returns></returns>
        public static DataTable GetErrorReport(int? uploadLogID)
        {
            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            ReportsTableAdapters.udf_GetErrorReportTableAdapter rpt = new ReportsTableAdapters.udf_GetErrorReportTableAdapter();
            rpt.Connection = ctx.GetSqlConnection();
            var dt = rpt.GetData(uploadLogID ?? 0);
            return dt;
        }

        /// <summary>
        /// Get Changes report data
        /// </summary>
        /// <param name="uploadLogID"></param>
        /// <returns></returns>
        public static DataTable GetChangesReport(int? uploadLogID)
        {
            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            ReportsTableAdapters.udf_GetChangesReportTableAdapter rpt = new ReportsTableAdapters.udf_GetChangesReportTableAdapter();
            rpt.Connection = ctx.GetSqlConnection();
            var dt = rpt.GetData(uploadLogID ?? 0);
            return dt;
        }

        /// <summary>
        /// Get Data Dictionary report
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDataDictionaryReport()
        {
            EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            ReportsTableAdapters.udf_GetDataDictionaryTableAdapter rpt = new ReportsTableAdapters.udf_GetDataDictionaryTableAdapter();
            rpt.Connection = ctx.GetSqlConnection();
            var dt = rpt.GetData();
            return dt;
        }
    }
}