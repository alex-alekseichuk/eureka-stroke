using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Reflection;

using Eureka.Stroke;
using Eureka.Stroke.Parser;
using Eureka.Stroke.UploaderService;
using Eureka.Stroke.FormatService;

namespace WebApplication
{
    /// <summary>
    /// Representation of UploadLog record in JSON
    /// </summary>
    [Serializable]
    [DataContract]
    public class JsonUploadLog
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public string UploadDate { get; set; }

        [DataMember]
        public int ErrorCount { get; set; }

        [DataMember]
        public int CorrectionsCount { get; set; }

        [DataMember]
        public int ChangesCount { get; set; }

        [DataMember]
        public short Status { get; set; }
    }

    /// <summary>
    /// Representation of Facility record in JSON
    /// </summary>
    [Serializable]
    [DataContract]
    public class JsonFacility
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }

    /// <summary>
    /// Summary description for UploaderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class UploaderWebService : System.Web.Services.WebService
    {
        public UploaderWebService()
        {
        }

        /// <summary>
        /// Static method to get UploadLog in web format.
        /// Convertion DB record to JSON format.
        /// </summary>
        /// <returns>list of UploadLog records in web format</returns>
        public static List<JsonUploadLog> GetUploadLogList()
        {
            var uploaderService = Eureka.Stroke.Container.DIContainer.Resolve<IUploaderService>(); 
            return uploaderService.GetUploadLogs().Select(uploadLog => new JsonUploadLog()
                                                {
                                                    ID = uploadLog.ID,
                                                    Filename = uploadLog.OriginalFilename,
                                                    UploadDate = (uploadLog.StartedDate ?? DateTime.Now).ToString(),
                                                    ErrorCount = uploadLog.ErrorCount ?? 0,
                                                    CorrectionsCount = uploadLog.CorrectionsCount ?? 0,
                                                    ChangesCount = uploadLog.ChangesCount ?? 0,
                                                    Status = (short) (uploadLog.Status ?? 0)
                                                }).ToList();
        }

        /// <summary>
        /// AJAX WebService method to check if such facility name is free to be added
        /// </summary>
        /// <param name="FacilityName"></param>
        /// <returns>Empty string if the name is free to be added or error message in other case</returns>
        [WebMethod]
        public string CheckFacilityName(string FacilityName)
        {
            var uploaderService = new UploaderService();
            return uploaderService.IsFacilityExisting(FacilityName) ? string.Format("Facility '{0}' is already registered", FacilityName) : string.Empty;
        }

        /// <summary>
        /// AJAX web method to get the list of Facility records
        /// </summary>
        /// <returns>The list of Facility records in JSON</returns>
        [WebMethod]
        public List<JsonFacility> GetFacilities()
        {
            var uploaderService = Eureka.Stroke.Container.DIContainer.Resolve<IUploaderService>();
            return uploaderService.GetFacilities(null).Select(facility => new JsonFacility()
            {
                ID = facility.ID,
                Name = facility.Name
            }).ToList();
        }

        /// <summary>
        /// AJAX web method to get the list of UploadLog records
        /// </summary>
        /// <returns>The list of UploadLog records in JSON</returns>
        [WebMethod]
        public List<JsonUploadLog> GetUploadLog()
        {
            return GetUploadLogList();
        }

        /// <summary>
        /// AJAX web method to upload
        /// </summary>
        /// <returns>+{Success message} or -{Error message}</returns>
        [WebMethod]
        public string Upload()
        {
            var context = HttpContext.Current;

            try
            {

                var mode = context.Request.Params["Mode"];
                var formatID = context.Request.Params["FormatID"].ToInt();
                var facilityID = context.Request.Params["FacilityID"].ToInt();
                var facilityName = context.Request.Params["FacilityName"];

                // get first uploaded file
                HttpPostedFile fileSource;
                if (null == context.Request.Files
                    || 0 == context.Request.Files.Count 
                    || null == (fileSource = context.Request.Files[0])
                    || string.IsNullOrEmpty(fileSource.FileName)
                    || fileSource.ContentLength == 0)
                {
                    Logger.Instance.LogInfo(string.Format("No file to upload or it is emtpy."));
                    return "-The file is empty or not properly formatted. The error is logged and will be reviewed by our team.";
                }

                // get uploader service
                var uploaderService = Eureka.Stroke.Container.DIContainer.Resolve<IUploaderService>();
                uploaderService.Upload(
                    mode.Equals("new") ? UploadMode.New : UploadMode.Existing,
                    facilityID,
                    formatID,
                    facilityName,
                    fileSource);

                Logger.Instance.LogInfo(string.Format("Uploading file {0}: {1}", fileSource.FileName, uploaderService.ErrorMessage));

                switch (uploaderService.ErrorCode)
                {
                    case 0:
                        return "+" + uploaderService.ErrorMessage;
                    case 3:
                    case 4:
                        return "-" + uploaderService.ErrorMessage;
                    default:
                        return "-Internal server error occurred. The error is logged and will be reviewed by our team.";
                }
            }
            catch (System.Web.HttpException exceptionHttp)
            {
                Logger.Instance.LogExceptionRecursively("Uploading", exceptionHttp, MethodBase.GetCurrentMethod());
                return "-The file is too big or not properly formatted. The error is logged and will be reviewed by our team.";
            }
        }
    }
}
