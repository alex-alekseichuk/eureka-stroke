using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Eureka.Stroke.FormatService;
using Eureka.Stroke.Parser;

using Eureka.Stroke.Data;

namespace Eureka.Stroke.UploaderService
{
    /// <summary>
    /// Implementation of file File Uploader services.
    /// It's used by File Uploader UI but maybe used by module as well.
    /// </summary>
    public class UploaderService : BaseService, IUploaderService
    {
        /// <summary>
        /// Get all facilities or get some sub-set filtered by query string
        /// </summary>
        /// <param name="queryString">If null or empty then get all facilities</param>
        /// <returns></returns>
        public IList<Facility> GetFacilities(string queryString)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // if no query then get all facilities
            if (string.IsNullOrEmpty(queryString))
                return DataFacade.GetFacilities(ctx);

            return DataFacade.FilterFacilities(ctx, queryString);
        }

        /// <summary>
        /// Add new facility under specified Format
        /// </summary>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public Facility AddFacility(string name, Format format)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == format)
                throw new ArgumentOutOfRangeException("format");

            if (string.IsNullOrEmpty(name))
            {
                ErrorCode = 2;
                ErrorMessage = "Facility Name is not provided";
                return null;
            }
            if (name.Length > 100)
            {
                ErrorCode = 3;
                ErrorMessage = "Facility Name may have length max 100 chars";
                return null;
            }

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>(); 

            if (null != DataFacade.GetFacilityByName(ctx, name))
            {
                ErrorCode = 1;
                ErrorMessage = "Facility with such Name already exists.";
                return null;
            }

            return DataFacade.AddFacility(ctx, name, format);
        }

        /// <summary>
        /// Check if Facility with specified name already existing
        /// </summary>
        /// <param name="name">Name of facility to check</param>
        /// <returns>true if such facility name alrady registered</returns>
        public bool IsFacilityExisting(string name)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            return null != DataFacade.GetFacilityByName(ctx, name);
        }

        /// <summary>
        /// Get existing Facility entity by its ID
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public Facility GetFacility(int facilityID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            return DataFacade.GetFacilityByID(ctx, facilityID);
        }

        /// <summary>
        /// Method to get all uploads to show them in File Uploader UI
        /// </summary>
        /// <returns>List of uploads</returns>
        public IList<UploadLog> GetUploadLogs()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            return DataFacade.GetUploadLogs(ctx);
        }

        #region upload

        /// <summary>
        /// Main method to upload data files via web interface.
        /// For New Facility mode it creates new facility first by specified Facility Name and Format ID.
        /// For Existing facility mode it loads existing Facility of specified Facility ID.
        /// </summary>
        /// <param name="mode">New Facility or Existing Facility mode</param>
        /// <param name="facilityID"></param>
        /// <param name="formatID"></param>
        /// <param name="facilityName"></param>
        /// <param name="fileSource">Uploaded via web file</param>
        public void Upload(UploadMode mode, int facilityID, int formatID, string facilityName, HttpPostedFile fileSource)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            Facility facility;

            if (mode == UploadMode.New)
            {
                if (string.IsNullOrEmpty(facilityName))
                {
                    ErrorCode = 3;
                    ErrorMessage = "Facility Name is not provided";
                    return;
                }
                if (facilityName.Length > 100)
                {
                    ErrorCode = 4;
                    ErrorMessage = "Facility Name may have length max 100 chars";
                    return;
                }

                var format = DataFacade.GetFormatByID(ctx, formatID);
                if (null == format)
                {
                    ErrorCode = 1;
                    ErrorMessage = "Incorrect Format";
                    return;
                }

                try
                {
                    facility = DataFacade.AddFacility(ctx, facilityName, format);
                }
                catch (Exception ex)
                {
                    var error = "Can't add new Facility " + facilityName;
                    Logger.Instance.LogException(error, ex);

                    ErrorCode = 5;
                    ErrorMessage = error;
                    return;
                }

                if (null == facility || 0 != ErrorCode)
                    return;
            }
            else // use existing facility for uploading
            {
                facility = DataFacade.GetFacilityByID(ctx, facilityID);
                if (null == facility)
                {
                    ErrorCode = 7;
                    ErrorMessage = "No Facility with such ID";
                    return;
                }
            }

            // construct unique file name
            var originalFileName = Path.GetFileName(fileSource.FileName);
            var fileName = string.Format("{0:yyyy-MM-dd_hhmmss}_{1}", DateTime.Now, originalFileName);
            var path = string.Format("{0}\\{1}", Config.Instance.UploaderFolder, fileName);

            // save uploaded file to specified folder
            try
            {
                fileSource.SaveAs(path);
            }
            catch (Exception ex)
            {
                var error = "Can't save file " + path;
                Logger.Instance.LogException(error, ex);

                ErrorCode = 2;
                ErrorMessage = error;
                return;
            }

            // create new UploadLog record
            UploadLog uploadLog;
            try
            {
                uploadLog = DataFacade.AddUploadLog(ctx, fileName, originalFileName, facility);
            }
            catch (Exception ex)
            {
                RemoveUploadedFile(path);

                var error = "Can't add new UploadLog " + facilityName;
                Logger.Instance.LogException(error, ex);

                ErrorCode = 6;
                ErrorMessage = error;
                return;
            }

            var parser = Container.DIContainer.Resolve<IParser>();

            // try to run parser process online
            var res = parser.Parse(Mode.Online, uploadLog);
            ErrorCode = parser.ErrorCode;
            ErrorMessage = parser.ErrorMessage;

            if (Result.NeedOffline == res)
            {
                // run parser offline as it woun't run online
                //res = parser.Parse(Mode.Offline, uploadLog);
                if (string.IsNullOrEmpty(ErrorMessage))
                    ErrorMessage = "Parsing process has been started in offline mode";
            }
            else if (Result.Done == res)
                ErrorMessage = "Done";
            return;
        }

        /// <summary>
        /// Remove uploaded but not well processed file.
        /// We should not have un-controlled files in upload folder.
        /// Files without appropriate record in UploadLog.
        /// </summary>
        /// <param name="path">Path to the file should be removed</param>
        private static void RemoveUploadedFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                var error = "Can't delete file " + path;
                Logger.Instance.LogException(error, ex);
            }
        }
        #endregion

    }
}
