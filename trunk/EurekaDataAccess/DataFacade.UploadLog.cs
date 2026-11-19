using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;

namespace Eureka.Stroke.Data
{
    public static partial class DataFacade
    {
        /// <summary>
        /// Get UploadLog records
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IList<UploadLog> GetUploadLogs(EurekaStrokeEntities ctx)
        {
            return (from uploadLog in ctx.UploadLog
                    orderby uploadLog.StartedDate descending
                    select uploadLog).ToList();
        }

        /// <summary>
        /// Get a list of pending upload logs (to be processed in offline mode)s
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static IList<UploadLog> GetPendingUploadLogs(EurekaStrokeEntities ctx)
        {
            return (from uploadLog in ctx.UploadLog.Include("Facility").Include("Facility.Format")
                    where uploadLog.Status == (int)Result.NeedOffline
                    orderby uploadLog.StartedDate ascending
                    select uploadLog).ToList();
        }

        /// <summary>
        /// Add new upload to the system for parsing.
        /// It's just added the record but not start the parsing.
        /// </summary>
        /// <param name="fileName">Name of file in Upload folder ready for parsing</param>
        /// <param name="originalFileName">Original name of the uploaded file</param>
        /// <param name="facility">Facility to be parsed under</param>
        /// <returns>UploadLog instance or null in case of error.</returns>
        public static UploadLog AddUploadLog(EurekaStrokeEntities ctx, string fileName, string originalFileName, Facility facility)
        {
            if (null == facility)
                throw new ArgumentOutOfRangeException("facility");

            // create new uploadLog record
            var uploadLog = new UploadLog
            {
                Filename = fileName,
                OriginalFilename = originalFileName,
                StartedDate = DateTime.Now,
                Facility = facility
            };

            ctx.AddToUploadLog(uploadLog);
            ctx.SaveChanges();

            return uploadLog;
        }

    }
}
