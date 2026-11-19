using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;
using ReportService.RS2005;

using System.Web.Services.Protocols;

namespace Eureka.Stroke.ReportService
{
    public class ReportService : BaseService, IReportService
    {

        #region Folder

        /// <summary>
        /// Get specified Folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public Folder GetFolderByID(int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFolderByID(ctx, folderID);
        }

        /// <summary>
        /// Get all folders in the stroke system
        /// </summary>
        /// <returns></returns>
        public IList<Folder> GetFolders()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFolders(ctx);
        }

        /// <summary>
        /// Get all folders available for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Folder> GetFoldersByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFoldersByUser(ctx, userID);
        }
        public IList<Folder> GetOtherFoldersByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetOtherFoldersByUser(ctx, userID);
        }

        /// <summary>
        /// Create new folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Folder AddFolder(string name)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.AddFolder(ctx, name);
        }

        /// <summary>
        /// Change the name of the folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="name">New folder name</param>
        public void UpdateFolder(int folderID, string name)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UpdateFolder(ctx, folderID, name);
        }

        /// <summary>
        /// Delete the folder and all its reports
        /// </summary>
        /// <param name="folderID"></param>
        public void DeleteFolder(int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.DeleteFolder(ctx, folderID);
        }

        /// <summary>
        /// Enable folder and all its enabled reports
        /// </summary>
        /// <param name="folderID"></param>
        public void EnableFolder(int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.EnableFolder(ctx, folderID, true);
        }

        /// <summary>
        /// Disable folder and all its reports
        /// </summary>
        /// <param name="folderID"></param>
        public void DisableFolder(int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.EnableFolder(ctx, folderID, false);
        }

        public void SortFoldersByName()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            ctx.SortFoldersByName();
        }

        #endregion

        #region Report
        /// <summary>
        /// Get All reports in the stroke system
        /// </summary>
        /// <returns></returns>
        public IList<Report> GetReports()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetReports(ctx);
        }

        /// <summary>
        /// Get all reports assigned for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Report> GetReportsByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetReportsByUser(ctx, userID);
        }

        /// <summary>
        /// Get all reports but not assigned for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Report> GetOtherReportsByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetOtherReportsByUser(ctx, userID);
        }

        /// <summary>
        /// Get both the all reports assigned for specified user + the reports of all assigned folders
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Report> GetAllReportsByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetAllReportsByUser(ctx, userID);
        }

        /// <summary>
        /// Get all reports in specified folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public IList<Report> GetFolderReports(int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFolderReports(ctx, folderID);
        }

        /// <summary>
        /// Get all reports of specified folder available for specified user
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Report> GetFolderReportsByUser(int folderID, int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFolderReportsByUser(ctx, folderID, userID);
        }

        /// <summary>
        /// Get specified by ID report
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        public Report GetReportByID(int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetReportByID(ctx, reportID);
        }

        /// <summary>
        /// Create new report
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Report AddReport(int folderID, string name, string description)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // make rsName as unique copy of name
            var rsName = name;
            var counter = 0;
            while (null != DataFacade.GetReportByRSName(ctx, rsName))
            {
                ++counter;
                rsName = name + DateTime.Now.ToString("yyyyMMddHHmmss") + counter;
            }

            return DataFacade.AddReport(ctx, folderID, name, description, rsName);
        }

        /// <summary>
        /// Update attributes of existing report
        /// </summary>
        /// <param name="reportID"></param>
        /// <param name="name">New name</param>
        /// <param name="description">New description</param>
        public Report UpdateReport(int reportID, string name, string description)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.UpdateReport(ctx, reportID, name, description);
        }

        public string GetReportPath(Report report)
        {
            if (null == report)
                return string.Format("{0}\\template\\template.rdl", Config.Instance.ReportsFolder);
            return string.Format("{0}\\report_{1}.rdl", Config.Instance.ReportsFolder, report.ID);
        }
        public void UploadReportFile(Report report, string tempPath)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var savedPath = GetReportPath(report);
            try
            {
                if (File.Exists(savedPath))
                    File.Delete(savedPath);
                File.Move(tempPath, savedPath);
                if (!UploadReport(report.RSName, savedPath, Config.Instance.ReportPath, true))
                {
                    ErrorCode = 2;
                    ErrorMessage = "Can't upload and save report file " + tempPath;
                }
            }
            catch (Exception e)
            {
                ErrorCode = 1;
                ErrorMessage = "Can't upload report file " + tempPath;
            }
        }

        protected bool UploadReport(string fileName, string filePath, string folderPath, bool overwrite)
        {
            
            var rs = new ReportingService2005();
            rs.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Byte[] definition = null;
            Warning[] warnings = null;
            
            try
            {
                FileStream stream = File.OpenRead(filePath);
                definition = new Byte[stream.Length];
                stream.Read(definition, 0, (int)stream.Length);
                stream.Close();
            }
            catch (IOException e)
            {
                //throw new Exception(e.Message);
                return false;
            }
            try
            {
                warnings = rs.CreateReport(fileName, folderPath, overwrite, definition, null);
                
                if (warnings != null)
                {
                    return false;
                    /*
                    foreach (Warning warning in warnings)
                    {
                        Console.WriteLine(warning.Message);
                    }
                    */
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete the report
        /// </summary>
        /// <param name="reportID"></param>
        public void DeleteReport(int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var report = DataFacade.GetReportByID(ctx, reportID);
            var savedPath = GetReportPath(report);

            DataFacade.DeleteReport(ctx, reportID);

            try
            {
                if (File.Exists(savedPath))
                    File.Delete(savedPath);
            }
            catch (Exception)
            {
                ErrorCode = 1;
                ErrorMessage = "Can't delete report file " + savedPath;
            }

        }

        /// <summary>
        /// Enable the report
        /// </summary>
        /// <param name="reportID"></param>
        public void EnableReport(int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.EnableReport(ctx, reportID, true);
        }

        /// <summary>
        /// Disable the report
        /// </summary>
        /// <param name="reportID"></param>
        public void DisableReport(int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.EnableReport(ctx, reportID, false);
        }

        public bool HasUserReport(int userID, int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.HasUserReport(ctx, userID, reportID);
        }

        #endregion

        #region Facility and ReportingSources

        /// <summary>
        /// Get all Facilities in the stroke system
        /// </summary>
        /// <returns></returns>
        public IList<Facility> GetFacilities()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFacilities(ctx);
        }

        /// <summary>
        /// Get all Reporting Sources in the stroke system
        /// </summary>
        /// <returns></returns>
        public IList<ReportingSource> GetReportingSources()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return (from s in DataFacade.GetReportingSources(ctx)
                    select new ReportingSource { Name = s }).ToList();
        }

        public IList<Facility> GetOtherFacilitiesByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetOtherFacilitiesByUser(ctx, userID);
        }

        public IList<ReportingSource> GetReportingSourcesByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return (from s in DataFacade.GetReportingSourcesByUser(ctx, userID)
                    select new ReportingSource {Name = s}).ToList();
        }
        public IList<ReportingSource> GetOtherReportingSourcesByUser(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return (from s in DataFacade.GetOtherReportingSourcesByUser(ctx, userID)
                    select new ReportingSource {Name = s}).ToList();
        }

        #endregion

        #region User privileges

        public bool HasUserReportingSource(int userID, string reportingSource)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.HasUserReportingSource(ctx, userID, reportingSource);
        }

        public IList<StrokeUser> GetUsers()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetUsers(ctx);
        }

        /// <summary>
        /// Get list of all object (all 4 types) that specified user has access.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<UserObject> GetUserObjects(int userID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var list = new List<UserObject>();

            (from f in DataFacade.GetDirectFoldersByUser(ctx, userID)
             select f).ForEach(f => list.Add(new UserObject { ID = f.ID, Name = f.Name, Type = UserObjectType.Folder }));

            (from r in DataFacade.GetReportsByUser(ctx, userID)
             select r).ForEach(r => list.Add(new UserObject { ID = r.ID, Name = r.Name, Type = UserObjectType.Report }));

            (from r in DataFacade.GetFacilitiesByUser(ctx, userID)
             select r).ForEach(r => list.Add(new UserObject { ID = r.ID, Name = r.Name, Type = UserObjectType.Facility }));

            (from str in DataFacade.GetReportingSourcesByUser(ctx, userID)
             select str).ForEach(str => list.Add(new UserObject { ID = 0, Name = str, Type = UserObjectType.ReportingSource }));

            return list;
        }

        public void UserGrant(int userID, UserObject userObject)
        {
            switch (userObject.Type)
            {
                case UserObjectType.Folder:
                    UserGrantFolder(userID, userObject.ID);
                    break;
                case UserObjectType.Report:
                    UserGrantReport(userID, userObject.ID);
                    break;
                case UserObjectType.Facility:
                    UserGrantFacility(userID, userObject.ID);
                    break;
                case UserObjectType.ReportingSource:
                    UserGrantReportingSource(userID, userObject.Name);
                    break;
            }
        }
        public void UserRevoke(int userID, UserObject userObject)
        {
            switch (userObject.Type)
            {
                case UserObjectType.Folder:
                    UserRevokeFolder(userID, userObject.ID);
                    break;
                case UserObjectType.Report:
                    UserRevokeReport(userID, userObject.ID);
                    break;
                case UserObjectType.Facility:
                    UserRevokeFacility(userID, userObject.ID);
                    break;
                case UserObjectType.ReportingSource:
                    UserRevokeReportingSource(userID, userObject.Name);
                    break;
            }
        }

        /// <summary>
        /// Grant permissions on the folder for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="folderID"></param>
        public void UserGrantFolder(int userID, int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserGrantFolder(ctx, userID, folderID);
        }

        /// <summary>
        /// Revoke permissions on the folder
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="folderID"></param>
        public void UserRevokeFolder(int userID, int folderID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserRevokeFolder(ctx, userID, folderID);
        }

        /// <summary>
        /// Grant permissions on the report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportID"></param>
        public void UserGrantReport(int userID, int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserGrantReport(ctx, userID, reportID);
        }

        /// <summary>
        /// Revoke permissions on the report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportID"></param>
        public void UserRevokeReport(int userID, int reportID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserRevokeReport(ctx, userID, reportID);
        }

        /// <summary>
        /// Grant permissions on facility
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="facilityID"></param>
        public void UserGrantFacility(int userID, int facilityID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserGrantFacility(ctx, userID, facilityID);
        }

        /// <summary>
        /// Revoke permissions on facility
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="facilityID"></param>
        public void UserRevokeFacility(int userID, int facilityID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserRevokeFacility(ctx, userID, facilityID);
        }

        /// <summary>
        /// Grant permissons on reporting source
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource"></param>
        public void UserGrantReportingSource(int userID, string reportingSource)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserGrantReportingSource(ctx, userID, reportingSource);
        }

        /// <summary>
        /// Revoke permissions on reporting source
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource"></param>
        public void UserRevokeReportingSource(int userID, string reportingSource)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            DataFacade.UserRevokeReportingSource(ctx, userID, reportingSource);
        }

        #endregion

    }
}
