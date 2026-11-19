using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Data
{
    /// <summary>
    /// Data access routines for Custom Reports module
    /// </summary>
    public static partial class DataFacade
    {

        #region Folder
        public static Folder GetFolderByID(EurekaStrokeEntities ctx, int folderID)
        {
            return (from folder in ctx.Folder
             where folder.ID == folderID
             select folder).FirstOrDefault();
        }

        public static IList<Folder> GetFolders(EurekaStrokeEntities ctx)
        {
            return (from folder in ctx.Folder
                    orderby folder.Rank, folder.Name
                    select folder).ToList();
        }

        public static IList<Folder> GetFoldersByUser(EurekaStrokeEntities ctx, int userID)
        {
            var queryReportsFolders = from userReport in ctx.UserReport
                     where userReport.UserID == userID && userReport.Report.Enabled && userReport.Report.Folder.Enabled
                     select userReport.Report.Folder;
            return (from userFolder in ctx.UserFolder
                    where userFolder.UserID == userID
                    orderby userFolder.Folder.Rank, userFolder.Folder.Name
                    select userFolder.Folder).Union(queryReportsFolders).Distinct().ToList();
        }
        public static IList<Folder> GetDirectFoldersByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from userFolder in ctx.UserFolder
                    where userFolder.UserID == userID
                    orderby userFolder.Folder.Rank, userFolder.Folder.Name
                    select userFolder.Folder).ToList();
        }
        public static IList<Folder> GetOtherFoldersByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from f in ctx.Folder
                    where f.Enabled && ! f.UserFolder.Any(uf => uf.UserID == userID && uf.Folder.Enabled)
                    orderby f.Rank, f.Name
                    select f).ToList();
        }

        public static Folder AddFolder(EurekaStrokeEntities ctx, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");

            // calculate the rank for this new folder
            var rank = (from f in ctx.Folder
                        orderby f.Rank descending 
                        select f.Rank).FirstOrDefault() + 1;

            var folder = Folder.CreateFolder(0, name, rank, true);
            ctx.AddToFolder(folder);
            ctx.SaveChanges();

            return folder;
        }

        public static void UpdateFolder(EurekaStrokeEntities ctx, int folderID, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");
            var folder = GetFolderByID(ctx, folderID);
            if (null == folder)
                throw new ArgumentOutOfRangeException("folderID");
            folder.Name = name;
            ctx.SaveChanges();
        }

        public static void EnableFolder(EurekaStrokeEntities ctx, int folderID, bool enabled)
        {
            var folder = GetFolderByID(ctx, folderID);
            if (null == folder)
                throw new ArgumentOutOfRangeException("folderID");
            folder.Enabled = enabled;
            ctx.SaveChanges();
        }

        public static void DeleteFolder(EurekaStrokeEntities ctx, int folderID)
        {
            var folder = GetFolderByID(ctx, folderID);
            if (null == folder)
                return;
            try
            {
                // delete UserFolder privileges
                (from uf in ctx.UserFolder
                 where uf.FolderID == folderID
                 select uf).ForEach(ctx.DeleteObject);

                // delete UserReport
                (from r in ctx.Report
                 where r.Folder.ID == folderID
                 select r).ForEach(r => DeleteReport(ctx, r));

                // delete the folder
                ctx.DeleteObject(folder);
                ctx.SaveChanges(true);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(string.Format("Error deleting folder '{0}'", folder.Name), ex);
            }

        }

        public static void SortFolders(EurekaStrokeEntities ctx)
        {
            
        }

        #endregion

        #region Report
        private static void DeleteReport(EurekaStrokeEntities ctx, Report report)
        {
            // delete UserReport privileges
            (from ur in ctx.UserReport
             where ur.ReportID == report.ID
             select ur).ForEach(ctx.DeleteObject);

            // delete the report
            ctx.DeleteObject(report);
        }

        public static void DeleteReport(EurekaStrokeEntities ctx, int reportID)
        {
            var report = GetReportByID(ctx, reportID);
            if (null == report)
                return;
            try
            {
                DeleteReport(ctx, report);
                ctx.SaveChanges(true);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(string.Format("Error deleting report '{0}'", report.Name), ex);
            }

        }

        public static Report GetReportByID(EurekaStrokeEntities ctx, int reportID)
        {
            return (from report in ctx.Report
                    where report.ID == reportID
                    select report).FirstOrDefault();
        }

        public static Report GetReportByRSName(EurekaStrokeEntities ctx, string rsName)
        {
            return (from report in ctx.Report
                    where report.RSName == rsName
                    select report).FirstOrDefault();
        }

        public static IList<Report> GetReports(EurekaStrokeEntities ctx)
        {
            return (from report in ctx.Report
                    select report).ToList();
        }
        /// <summary>
        /// Both the reports assigned to user directly + reports of assigned folders
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static IList<Report> GetAllReportsByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from ur in ctx.UserReport
                    join uf in ctx.UserFolder on ur.Report.Folder.ID equals uf.Folder.ID
                    where ur.UserID == userID && ur.Report.Enabled && uf.Folder.Enabled && uf.UserID == userID
                    select ur.Report).ToList();
        }

        /// <summary>
        /// Only reports assigned to user directly
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static IList<Report> GetReportsByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from ur in ctx.UserReport
                    where ur.UserID == userID && ur.Report.Enabled
                    select ur.Report).ToList();
        }
        public static IList<Report> GetOtherReportsByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from r in ctx.Report
                    where r.Enabled && ! r.UserReport.Any(ur => ur.UserID == userID && ur.Report.Enabled) 
                    select r).ToList();
        }
        
        public static IList<Report> GetFolderReports(EurekaStrokeEntities ctx, int folderID)
        {
            return (from r in ctx.Report
                    where r.Folder.ID == folderID
                    select r).ToList();
        }
        public static IList<Report> GetFolderReportsByUser(EurekaStrokeEntities ctx, int folderID, int userID)
        {
            
            return (from ur in ctx.UserReport
                    where ur.UserID == userID && ur.Report.Enabled && ur.Report.Folder.Enabled && ur.Report.Folder.ID == folderID
                    select ur.Report).ToList();
            /*
            var q1 = from ur in ctx.UserReport
                     where ur.UserID == userID && ur.Report.Enabled && ur.Report.Folder.Enabled && ur.Report.Folder.ID == folderID
                     select ur.Report;
            var q2 = from uf in ctx.UserFolder
                     where uf.UserID == userID && ur.Report.Enabled && ur.Report.Folder.Enabled && ur.Report.Folder.ID == folderID
                     select ur.Report;
            */
        }
        public static Report AddReport(EurekaStrokeEntities ctx, int folderID, string name, string description, string rsName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");

            var report = Report.CreateReport(0, name, true, rsName);
            report.Folder = (from f in ctx.Folder
                             where f.ID == folderID
                             select f).FirstOrDefault();
            report.Description = description;
            ctx.AddToReport(report);
            ctx.SaveChanges();

            return report;
        }
        public static Report UpdateReport(EurekaStrokeEntities ctx, int reportID, string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");

            var report = GetReportByID(ctx, reportID);
            if (null == report)
                throw new ArgumentOutOfRangeException("reportID");
            report.Name = name;
            report.Description = description;
            ctx.SaveChanges();
            return report;
        }

        public static void EnableReport(EurekaStrokeEntities ctx, int reportID, bool enabled)
        {
            var report = GetReportByID(ctx, reportID);
            if (null == report)
                throw new ArgumentOutOfRangeException("reportID");
            report.Enabled = enabled;
            ctx.SaveChanges();
        }

        public static bool HasUserReport(EurekaStrokeEntities ctx, int userID, int reportID)
        {
            return (from ur in ctx.UserReport
                    where ur.ReportID == reportID && ur.UserID == userID
                    select ur).Count() > 0;
        }

        #endregion

        #region Facility and ReportingSource

        /*
        public static IList<Facility> GetFacilities(EurekaStrokeEntities ctx)
        {
            return (from f in ctx.Facility
                    select f).ToList();
        }
        */

        public static IList<Facility> GetFacilitiesByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from uf in ctx.UserFacility
                    where uf.UserID == userID
                    select uf.Facility).ToList();
        }
        public static IList<Facility> GetOtherFacilitiesByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from f in ctx.Facility
                    where !f.UserFacility.Any(uf => uf.UserID == userID)
                    select f).ToList();
        }

        public static IList<string> GetReportingSources(EurekaStrokeEntities ctx)
        {
            return (from sd in ctx.StrokeData
                    select sd.ReportingSource).Distinct().ToList();
        }

        public static IList<string> GetReportingSourcesByUser(EurekaStrokeEntities ctx, int userID)
        {
            return (from ur in ctx.UserReportingSource
                    where ur.UserID == userID
                    select ur.ReportingSource).ToList();
        }
        public static IList<string> GetOtherReportingSourcesByUser(EurekaStrokeEntities ctx, int userID)
        {
            var granted = (from urs in ctx.UserReportingSource
                           where urs.UserID == userID
                           select urs.ReportingSource).ToList();
            var all = (from sd in ctx.StrokeData
                       where !string.IsNullOrEmpty(sd.ReportingSource)
                       select sd.ReportingSource).Distinct().ToList();
            all.RemoveAll(granted.Contains);
            return all;
        }

        #endregion

        #region User

        public static IList<StrokeUser> GetUsers(EurekaStrokeEntities ctx)
        {
            return (from u in ctx.StrokeUser
                    select u).ToList();
        }

        /// <summary>
        /// Gues if specified user has access to specified reportingSource
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static bool HasUserReportingSource(EurekaStrokeEntities ctx, int userID, string reportingSource)
        {
            return (from urs in ctx.UserReportingSource
                        where urs.ReportingSource == reportingSource && urs.UserID == userID
                        select urs).Count() > 0;
        }

        public static void UserGrantFolder(EurekaStrokeEntities ctx, int userID, int folderID)
        {
            if (null == (from uf in ctx.UserFolder
                         where uf.FolderID == folderID && uf.UserID == userID
                         select uf).FirstOrDefault())
            {
                var userFolder = UserFolder.CreateUserFolder(folderID, userID);
                ctx.AddToUserFolder(userFolder);
                userFolder.Folder = GetFolderByID(ctx, folderID);
                //GetFolderByID(ctx, folderID).UserFolder.Add(userFolder);
                ctx.SaveChanges();
            }
        }
        public static void UserRevokeFolder(EurekaStrokeEntities ctx, int userID, int folderID)
        {
            var userFolder = (from uf in ctx.UserFolder
                              where uf.FolderID == folderID && uf.UserID == userID
                              select uf).FirstOrDefault();
            if (null != userFolder)
            {
                ctx.DeleteObject(userFolder);
                ctx.SaveChanges();
            }
        }

        public static void UserGrantReport(EurekaStrokeEntities ctx, int userID, int reportID)
        {
            if (null == (from uf in ctx.UserReport
                         where uf.ReportID == reportID && uf.UserID == userID
                         select uf).FirstOrDefault())
            {
                var userReport = UserReport.CreateUserReport(reportID, userID);
                ctx.AddToUserReport(userReport);
                userReport.Report = GetReportByID(ctx, reportID);
                //GetReportByID(ctx, reportID).UserReport.Add(userReport);
                ctx.SaveChanges();
            }
        }
        public static void UserRevokeReport(EurekaStrokeEntities ctx, int userID, int reportID)
        {
            var userReport = (from uf in ctx.UserReport
                              where uf.ReportID == reportID && uf.UserID == userID
                              select uf).FirstOrDefault();
            if (null != userReport)
            {
                ctx.DeleteObject(userReport);
                ctx.SaveChanges();
            }
        }

        public static void UserGrantFacility(EurekaStrokeEntities ctx, int userID, int facilityID)
        {
            if (null == (from uf in ctx.UserFacility
                         where uf.FacilityID == facilityID && uf.UserID == userID
                         select uf).FirstOrDefault())
            {
                var userFacility = UserFacility.CreateUserFacility(facilityID, userID);
                ctx.AddToUserFacility(userFacility);
                userFacility.Facility = GetFacilityByID(ctx, facilityID);
                //GetFacilityByID(ctx, facilityID).UserFacility.Add(userFacility);
                ctx.SaveChanges(true);
            }
        }
        public static void UserRevokeFacility(EurekaStrokeEntities ctx, int userID, int facilityID)
        {
            var userFacility = (from uf in ctx.UserFacility
                                where uf.FacilityID == facilityID && uf.UserID == userID
                                select uf).FirstOrDefault();
            if (null != userFacility)
            {
                ctx.DeleteObject(userFacility);
                ctx.SaveChanges();
            }
        }

        public static void UserGrantReportingSource(EurekaStrokeEntities ctx, int userID, string reportingSource)
        {
            if (null == (from uf in ctx.UserReportingSource
                         where uf.ReportingSource == reportingSource && uf.UserID == userID
                         select uf).FirstOrDefault())
            {
                var userReportingSource = UserReportingSource.CreateUserReportingSource(reportingSource, userID);
                ctx.AddToUserReportingSource(userReportingSource);
                ctx.SaveChanges();
            }
        }
        public static void UserRevokeReportingSource(EurekaStrokeEntities ctx, int userID, string reportingSource)
        {
            var userReportingSource = (from uf in ctx.UserReportingSource
                                       where uf.ReportingSource == reportingSource && uf.UserID == userID
                                select uf).FirstOrDefault();
            if (null != userReportingSource)
            {
                ctx.DeleteObject(userReportingSource);
                ctx.SaveChanges();
            }
        }

        #endregion

    }
}
