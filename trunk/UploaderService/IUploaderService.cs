using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Eureka.Stroke.UploaderService
{
    public enum UploadMode
    {
        New = 0,
        Existing = 1
    }

    /// <summary>
    /// Interface used to register new uploaded (or copied) files for parsing
    /// </summary>
    public interface IUploaderService : IError
    {
        /// <summary>
        /// Get full list of facilities or some search result by provided query string
        /// </summary>
        /// <param name="queryString">optional parameter to filter whole facilities list by criteria: Name like '%queryString%'</param>
        /// <returns>list of facilities</returns>
        IList<Facility> GetFacilities(string queryString);

        /// <summary>
        /// Add new facility
        /// </summary>
        /// <param name="name">Name of facility</param>
        /// <param name="format">Format of facility</param>
        Facility AddFacility(string name, Format format);

        /// <summary>
        /// Check if Facility with specified name already existing
        /// </summary>
        /// <param name="name">Name of facility to check</param>
        /// <returns>true if such facility name alrady registered</returns>
        bool IsFacilityExisting(string name);

        /// <summary>
        /// Add uploaded file to UploadLog for parsing.
        /// It's just added the record but not start the parsing.
        /// </summary>
        /// <param name="mode">Mode of the uploading process: New or Existing</param>
        /// <param name="facilityID">Facility ID used in Existing mode</param>
        /// <param name="formatID">Format ID used in New Mode</param>
        /// <param name="facilityName">New facility name used in New mode</param>
        /// <param name="fileSource">Http file uploaded via multipart POST request</param>
        /// <returns>Added UploadLog record or null</returns>
        void Upload(UploadMode mode, int facilityID, int formatID, string facilityName, HttpPostedFile fileSource);

        /// <summary>
        /// Method to get all uploads to show them in File Uploader UI
        /// </summary>
        /// <returns>List of uploads</returns>
        IList<UploadLog> GetUploadLogs();

        /// <summary>
        /// Get Facility by ID
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        Facility GetFacility(int facilityID);
    }
}
