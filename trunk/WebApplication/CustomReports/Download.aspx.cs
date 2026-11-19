using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Eureka.Stroke;
using Eureka.Stroke.ReportService;

namespace WebApplication.CustomReports
{
    public partial class Download : StrokeBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Util.IsAdmin())
                Response.Redirect("AdminReports.aspx");

            var reportID = Request.Params["reportID"].ToInt();
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            var path = "";
            var fileName = "";
            if (0 != reportID)
            {
                var report = reportService.GetReportByID(reportID);
                if (null == report)
                    return404();
                path = reportService.GetReportPath(report);
                fileName = report.Name + ".rdl";
            }
            else
            {
                path = reportService.GetReportPath(null);
                fileName = "template.rdl";
            }
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return404();

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileName));
            Response.WriteFile(path);
        }
        protected void return404()
        {
            Response.AppendHeader("Content-Length", "0");
            Response.StatusCode = 404;
            Response.StatusDescription = "Not Found";
            Response.End();
        }
    }
}
