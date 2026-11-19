using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Data;
using Eureka.Stroke;
using Eureka.Stroke.Data;

public partial class AuditReports : System.Web.UI.Page
{

    private const string ErrorReport = "ErrorReport";
    private const string ChangesReport = "CorrectionReport";
    private const string DataDictionaryReport = "DataDictionaryReport";

    /// <summary>
    /// Stores current report
    /// </summary>
    private string CurrentReport
    {
        get
        {
            string report = ErrorReport;
            try
            {
                report = (string)ViewState["CurrentReport"];
            }
            catch { }
            return report;
        }
        set
        {
            ViewState["CurrentReport"] = value;
        }
    }

    /// <summary>
    /// Ger Report name by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GetReportByID(int id)
    {
        string repName = ErrorReport;
        switch (id)
        {
            case 1:
                repName = ErrorReport; break;
            case 2:
                repName = ChangesReport; break;
            case 3:
                repName = DataDictionaryReport; break;
        }
        return repName;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindUploadLog();
            


        }
    }

    protected int? GetUploadLogID()
    {
        int? reportParameter = drpUploadLog.Items.Count > 0 ? Convert.ToInt32(drpUploadLog.SelectedValue) : 0;
        return reportParameter;
    }
   

    protected void rptViewer_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int reportID = 1;
            try
            {
                string rid = Request.QueryString["rid"];
                reportID = int.Parse(rid);
            }
            catch { }

            int? uploadID = 0;
            try
            {
                string uid = Request.QueryString["uid"];
                uploadID = int.Parse(uid);
                drpUploadLog.SelectedValue = uploadID.ToString();
            }
            catch { }
            
            string reportName = GetReportByID(reportID);
            int? reportParameter = GetUploadLogID();
            ReportGenerator(reportName, reportParameter);
        }
        else rptViewer.Reset(); 
    }

    private void ReportGenerator(string reportName, int? reportParameter)
    {       
        string path = HttpContext.Current.Server.MapPath("~/Reports/");
        CurrentReport = reportName;

        rptViewer.Reset(); 
        rptViewer.ProcessingMode = ProcessingMode.Local;
        LocalReport r = rptViewer.LocalReport;
        r.ReportPath = path + reportName + ".rdlc";
        ReportDataSource reportDS;
        switch (reportName)
        {
            case ErrorReport:
                reportDS = new ReportDataSource("Reports_udf_GetErrorReport", ReportHelper.GetErrorReport(reportParameter));
                rptViewer.LocalReport.DataSources.Add(reportDS);
                rptViewer.LocalReport.Refresh();
                
                break;
            case ChangesReport:
                reportDS = new ReportDataSource("Reports_udf_GetChangesReport", ReportHelper.GetChangesReport(reportParameter));
                rptViewer.LocalReport.DataSources.Add(reportDS);
                rptViewer.LocalReport.Refresh();
                break;

            case DataDictionaryReport:
                reportDS = new ReportDataSource("Reports_udf_GetDataDictionary", ReportHelper.GetDataDictionaryReport());
                rptViewer.LocalReport.DataSources.Add(reportDS);
                rptViewer.LocalReport.Refresh();
                break;
        }
       
    }

    private void BindUploadLog()
    {
        EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
        var logs = DataFacade.GetUploadLogs(ctx);
        //ctx.Connection;
        drpUploadLog.DataSource = logs;
        drpUploadLog.DataTextField = "FriendlyName";
        drpUploadLog.DataValueField = "Id";
        drpUploadLog.DataBind();

    }

    protected void drpUploadLog_SelectedIndexChanged(object sender, EventArgs e)
    {
        //BindUploadLog();
        int? reportParameter = GetUploadLogID();
        ReportGenerator(CurrentReport, reportParameter);
    }

    protected void btnErrorReport_Click(object sender, EventArgs e)
    {
        int? reportParameter = GetUploadLogID();
        ReportGenerator(ErrorReport, reportParameter);
    }

    protected void btnChangesReport_Click(object sender, EventArgs e)
    {
        int? reportParameter = GetUploadLogID();
        ReportGenerator(ChangesReport, reportParameter);
    }
    protected void btnDataDictionary_Click(object sender, EventArgs e)
    {
        ReportGenerator(DataDictionaryReport,null);
    }
}
