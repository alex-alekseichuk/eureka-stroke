using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Eureka.Stroke;
using Eureka.Stroke.ReportService;

namespace WebApplication.CustomReports
{
    public class UsersDataSource
    {
        public IList<StrokeUser> GetUsers()
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetUsers();
        }
    }
    public class UserObjectsDataSource
    {
        public IList<UserObject> GetUserObjects(int userID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetUserObjects(userID);
        }
        public void DeleteUserObject(int userID, int ID, UserObjectType type, String name)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UserRevoke(userID, new UserObject() {ID=ID, Type = type, Name = name});
        }

        public IList<Folder> GetOtherFolders(int userID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetOtherFoldersByUser(userID);
        }
        public IList<Report> GetOtherReports(int userID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetOtherReportsByUser(userID);
        }
        public IList<Facility> GetOtherFacilities(int userID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetOtherFacilitiesByUser(userID);
        }
        public IList<ReportingSource> GetOtherReportingSources(int userID)
        {
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            return reportService.GetOtherReportingSourcesByUser(userID);
        }

    }



    public partial class UsersAdmin : CustomReportControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Util.IsAdmin())
                Response.Redirect("AdminReports.aspx");
        }

        protected void listUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            phUserPrivileges.Visible = true;
        }

        protected void listUserObjects_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            listOtherFolders.DataBind();
            listOtherReports.DataBind();
            listOtherFacilities.DataBind();
            listOtherReportingSources.DataBind();
        }

        protected void listOtherFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = listOtherFolders.SelectedDataKey["ID"];
            listOtherFolders.SelectedIndex = -1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UserGrantFolder(listUsers.SelectedDataKey["ID"].ToInt(), id.ToInt());
            listUserObjects.DataBind();
        }
        protected void listOtherReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = listOtherReports.SelectedDataKey["ID"];
            listOtherReports.SelectedIndex = -1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UserGrantReport(listUsers.SelectedDataKey["ID"].ToInt(), id.ToInt());
            listUserObjects.DataBind();
        }
        protected void listOtherFacilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = listOtherFacilities.SelectedDataKey["ID"];
            listOtherFacilities.SelectedIndex = -1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UserGrantFacility(listUsers.SelectedDataKey["ID"].ToInt(), id.ToInt());
            listUserObjects.DataBind();
        }
        protected void listOtherReportingSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = listOtherReportingSources.SelectedDataKey["Name"].ToString();
            listOtherReportingSources.SelectedIndex = -1;
            var reportService = new Eureka.Stroke.ReportService.ReportService();
            reportService.UserGrantReportingSource(listUsers.SelectedDataKey["ID"].ToInt(), name);
            listUserObjects.DataBind();
        }


    }
}