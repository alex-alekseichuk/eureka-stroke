using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.ReportService
{
    public interface IReportService
    {
        #region Folder

        /// <summary>
        /// Get specified Folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        Folder GetFolderByID(int folderID);

        /// <summary>
        /// Get all folders in the stroke system
        /// </summary>
        /// <returns></returns>
        IList<Folder> GetFolders();

        /// <summary>
        /// Get all folders available for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IList<Folder> GetFoldersByUser(int userID);
        IList<Folder> GetOtherFoldersByUser(int userID);

        /// <summary>
        /// Create new folder
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Folder AddFolder(string name);

        /// <summary>
        /// Change the name of the folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="name">New folder name</param>
        void UpdateFolder(int folderID, string name);

        /// <summary>
        /// Delete the folder and all its reports
        /// </summary>
        /// <param name="folderID"></param>
        void DeleteFolder(int folderID);

        /// <summary>
        /// Enable folder and all its enabled reports
        /// </summary>
        /// <param name="folderID"></param>
        void EnableFolder(int folderID);

        /// <summary>
        /// Disable folder and all its reports
        /// </summary>
        /// <param name="folderID"></param>
        void DisableFolder(int folderID);

        void SortFoldersByName();

        #endregion

        #region Report

        /// <summary>
        /// Get All reports in the stroke system
        /// </summary>
        /// <returns></returns>
        IList<Report> GetReports();

        /// <summary>
        /// Get all reports available for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IList<Report> GetReportsByUser(int userID);
        IList<Report> GetOtherReportsByUser(int userID);

        /// <summary>
        /// Get all reports in specified folder
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        IList<Report> GetFolderReports(int folderID);

        /// <summary>
        /// Get all reports of specified folder available for specified user
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        IList<Report> GetFolderReportsByUser(int folderID, int userID);

        /// <summary>
        /// Get specified by ID report
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        Report GetReportByID(int reportID);

        /// <summary>
        /// Create new report
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        Report AddReport(int folderID, string name, string description);

        /// <summary>
        /// Update attributes of existing report
        /// </summary>
        /// <param name="reportID"></param>
        /// <param name="name">New name</param>
        /// <param name="description">New description</param>
        Report UpdateReport(int reportID, string name, string description);

        void UploadReportFile(Report report, string tempPath);

        /// <summary>
        /// Delete the report
        /// </summary>
        /// <param name="reportID"></param>
        void DeleteReport(int reportID);

        /// <summary>
        /// Enable the report
        /// </summary>
        /// <param name="reportID"></param>
        void EnableReport(int reportID);

        /// <summary>
        /// Disable the report
        /// </summary>
        /// <param name="reportID"></param>
        void DisableReport(int reportID);

        #endregion

        #region Facility and ReportingSource

        /// <summary>
        /// Get all Facilities in the stroke system
        /// </summary>
        /// <returns></returns>
        IList<Facility> GetFacilities();

        /// <summary>
        /// Get all Reporting Sources in the stroke system
        /// </summary>
        /// <returns></returns>
        IList<ReportingSource> GetReportingSources();

        IList<Facility> GetOtherFacilitiesByUser(int userID);
        IList<ReportingSource> GetOtherReportingSourcesByUser(int userID);

        #endregion

        #region User

        IList<StrokeUser> GetUsers();

        /// <summary>
        /// Check if specified user able to use specified reporting source
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource"></param>
        /// <returns></returns>
        bool HasUserReportingSource(int userID, string reportingSource);

        /// <summary>
        /// Get list of all object (all 4 types) that specified user has access.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IList<UserObject> GetUserObjects(int userID);


        void UserGrant(int userID, UserObject userObject);
        void UserRevoke(int userID, UserObject userObject);

        /// <summary>
        /// Grant permissions on the folder for specified user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="folderID"></param>
        void UserGrantFolder(int userID, int folderID);

        /// <summary>
        /// Revoke permissions on the folder
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="folderID"></param>
        void UserRevokeFolder(int userID, int folderID);

        /// <summary>
        /// Grant permissions on the report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportID"></param>
        void UserGrantReport(int userID, int reportID);

        /// <summary>
        /// Revoke permissions on the report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportID"></param>
        void UserRevokeReport(int userID, int reportID);

        /// <summary>
        /// Grant permissions on facility
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="facilityID"></param>
        void UserGrantFacility(int userID, int facilityID);

        /// <summary>
        /// Revoke permissions on facility
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="facilityID"></param>
        void UserRevokeFacility(int userID, int facilityID);

        /// <summary>
        /// Grant permissons on reporting source
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource"></param>
        void UserGrantReportingSource(int userID, string reportingSource);

        /// <summary>
        /// Revoke permissions on reporting source
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource"></param>
        void UserRevokeReportingSource(int userID, string reportingSource);

        #endregion
    }
}
