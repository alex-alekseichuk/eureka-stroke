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
    public class FoldersDataSource
    {
        public IList<Folder> GetFolders()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            if (Util.IsAdmin())
                return reportService.GetFolders() as IList<Folder>;
            return reportService.GetFoldersByUser(Util.UserID());
        }
        public void UpdateFolder(int id, string name, bool enabled)
        {
            if (!Util.IsAdmin())
                return;
            if (string.IsNullOrEmpty(name))
                return;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UpdateFolder(id, name);
            if (enabled)
                reportService.EnableFolder(id);
            else
                reportService.DisableFolder(id);
        }
        public Folder InsertFolder(string name, bool enabled)
        {
            if (!Util.IsAdmin())
                return null;
            if (string.IsNullOrEmpty(name))
                return null;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            var folder = reportService.AddFolder(name);
            if (enabled)
                reportService.EnableFolder(folder.ID);
            else
                reportService.DisableFolder(folder.ID);
            return folder;
        }
        public void DeleteFolder(int id)
        {
            if (!Util.IsAdmin())
                return;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.DeleteFolder(id);
        }

    }

    public class ReportsDataSource
    {
        public IList<Report> GetReports(int folderID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            if (Util.IsAdmin())
                return reportService.GetFolderReports(folderID) as IList<Report>;
            return reportService.GetFolderReportsByUser(folderID, Util.UserID());
        }
        public Report InsertReport(int folderID, string name, bool enabled, string description, string tempPath)
        {
            if (!Util.IsAdmin())
                return null;
            Report report = null;
            if (!string.IsNullOrEmpty(name))
            {

                var reportService = new Eureka.Stroke.ReportService.ReportService();
                report = reportService.AddReport(folderID, name, description);
                _UpdateReport(reportService, report, enabled, tempPath);
            }
            else
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            return report;
        }
        public Report UpdateReport(int id, string name, bool enabled, string description, string tempPath)
        {
            if (!Util.IsAdmin())
                return null;
            Report report = null;
            if (!string.IsNullOrEmpty(name))
            {
                var reportService = new Eureka.Stroke.ReportService.ReportService();
                report = reportService.UpdateReport(id, name, description);
                _UpdateReport(reportService, report, enabled, tempPath);
            } else
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            return report;
        }
        protected void _UpdateReport(IReportService reportService, Report report, bool enabled, string tempPath)
        {
            if (enabled)
                reportService.EnableReport(report.ID);
            else
                reportService.DisableReport(report.ID);
            if (!string.IsNullOrEmpty(tempPath))
            {
                reportService.UploadReportFile(report, tempPath);
            }
        }
        public void DeleteReport(int id)
        {
            if (!Util.IsAdmin())
                return;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.DeleteReport(id);
        }

    }

    public partial class ReportsAdmin : CustomReportControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.GetPostBackEventReference(this, "");
            if (!Util.IsAdmin())
            {
                listFolders.InsertItemPosition = InsertItemPosition.None;
                listReports.InsertItemPosition = InsertItemPosition.None;
                divDownloadTemplate.Visible = false;

                var btnSort = (Button)listFolders.FindControlRecursive("btnSort");
                if (null != btnSort)
                    btnSort.Visible = false;

            }
        }

        protected void listFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            phReports.Visible = true;
        }



        private bool ReportUploading(string fileUploadID, ObjectDataSourceMethodEventArgs e)
        {
            var file = (FileUpload)listReports.FindControlRecursive(fileUploadID);
            if (null == file)
                return false;
            if (file.HasFile)
            {
                var path = string.Format("{0}\\temp\\{1}", Config.Instance.ReportsFolder, file.FileName);
                try
                {
                    file.SaveAs(path);
                    e.InputParameters["tempPath"] = path;
                    return true;
                }
                catch (Exception)
                {
                    e.Cancel = true;
                }
            }
            return false;
        }

        protected void listReports_Inserting(Object sender, ObjectDataSourceMethodEventArgs e)
        {
            if (!ReportUploading("fileUpdateReport", e))
                e.Cancel = true;
        }
        protected void listReports_Updating(Object sender, ObjectDataSourceMethodEventArgs e)
        {
            ReportUploading("fileUpdateReport", e);
        }

        protected void btnSort_Click(object sender, EventArgs e)
        {
            if (!Util.IsAdmin())
                return;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.SortFoldersByName();
            listFolders.DataBind();
        }

    }
}