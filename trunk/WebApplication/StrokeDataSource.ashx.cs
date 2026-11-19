using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Eureka.Stroke.Data;
using Eureka.Stroke;
using Eureka.Stroke.ReportService;

namespace WebApplication
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class StrokeDataSource : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";

            var userID = 1; // TODO provide real user ID here instead of hardcode
            var reportingSource = context.Request.Params["rs"];

            var reportService = new Eureka.Stroke.ReportService.ReportService();

            // check if specified user has privileges for this reporting source
            if (!reportService.HasUserReportingSource(userID, reportingSource))
                return;

            var data = DataFacade.GetPivotReportData(userID, reportingSource);
            data.WriteXml(context.Response.OutputStream);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
