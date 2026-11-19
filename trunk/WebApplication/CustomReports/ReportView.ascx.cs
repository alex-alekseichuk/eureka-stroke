using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Eureka.Stroke;
using Eureka.Stroke.ReportService;
using Microsoft.Reporting.WebForms;

namespace WebApplication.CustomReports
{
    public class ReportingSourcesDataSource
    {
        public IList<ReportingSource> GetReportingSources()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            if (Util.IsAdmin())
                return reportService.GetReportingSources();
            return reportService.GetReportingSourcesByUser(Util.UserID());
        }
    }

    public partial class ReportView : CustomReportControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var reportID = Request.Params["reportID"].ToInt();
                var reportService = new Eureka.Stroke.ReportService.ReportService();

                if (!Util.IsAdmin())
                {
                    if (!reportService.HasUserReport(Util.UserID(), reportID))
                    {
                        Response.AppendHeader("Content-Length", "0");
                        Response.StatusCode = 401;
                        Response.StatusDescription = "Unauthorized";
                        Response.End();
                        //Response.Redirect("AdminReports.aspx");
                    }
                }

                var report = reportService.GetReportByID(reportID);
                if (null == report)
                {
                    Response.AppendHeader("Content-Length", "0");
                    Response.StatusCode = 404;
                    Response.StatusDescription = "Not found";
                    Response.End();
                }

                lblReportDescription.Text = report.Description;

                //rvControl.ServerReport.ReportServerUrl = new Uri(GlobalConst.ReportServerUrl);
                //rvControl.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer_SS2008");
                rvControl.ServerReport.ReportServerUrl = new Uri(Config.Instance.ReportsServerUrl);
                rvControl.ServerReport.ReportPath = Config.Instance.ReportPath + "/" + report.RSName;
                //rvControl.ServerReport.ReportServerCredentials = new ReportServerCredential();


            }
        }
    }

    [Serializable]
    public class ReportServerCredential : IReportServerCredentials
    {

        public static string SSRSUserName
        {
            get { return ConfigurationManager.AppSettings.Get("SSRSUserName"); }
        }

        public static string SSRSUserPassword
        {
            get { return ConfigurationManager.AppSettings.Get("SSRSUserPassword"); }
        }

        public static string SSRSUserDomain
        {
            get { return ConfigurationManager.AppSettings.Get("SSRSUserDomain"); }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string userName, out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;
            return false;
        }

        public WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public ICredentials NetworkCredentials
        {
            get { return new NetworkCredential(SSRSUserName, SSRSUserPassword, SSRSUserDomain); }
        }
    }

    public class GlobalConst
    {

        public static string ReportServerUrl
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("ReportServerUrl"); // with end slash
            }
        }

        public static string ReportServer
        {
            get { return ConfigurationManager.AppSettings.Get("ReportServerUrl"); }
        }

        public static string ReportPath
        {
            get { return ConfigurationManager.AppSettings.Get("ReportPath"); }
        }

        public static char[] PathSeparatorArray
        {
            get { return pathSeparatorArray; }
        }

        public static string PathSeparatorString
        {
            get { return pathSeparatorString; }
        }

        public static CredentialCache SSRSCredential
        {
            get
            {
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(ReportServer), "NTLM", (NetworkCredential)new ReportServerCredential().NetworkCredentials);
                return credentialCache;
            }
        }

        private const char pathSeparator = '/';
        static readonly char[] pathSeparatorArray = { pathSeparator };
        static readonly string pathSeparatorString = new string(pathSeparator, 1);
    }

}