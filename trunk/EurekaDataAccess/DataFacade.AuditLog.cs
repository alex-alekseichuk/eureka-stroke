using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;
using System.Data;
using System.Data.Objects;

namespace Eureka.Stroke.Data
{
    public static partial class DataFacade
    {
        /// <summary>
        /// Create AuditLog record
        /// </summary>
        /// <param name="res"></param>
        /// <param name="isValid"></param>
        /// <param name="strokeValue"></param>
        /// <param name="uploadLog"></param>
        /// <param name="fieldName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static AuditLog CreateAuditLog(EurekaStrokeEntities ctx, int res, bool isValid, StrokeFieldValue strokeValue, UploadLog uploadLog, string fieldName, string errorMessage, int rowNumber)
        {
            object user = DataFacade.GetCurrentUser();
            AuditLog log = AuditLog.CreateAuditLog(0, (short)EnumActionType.Create, DateTime.Now, user == null ? 0 : (int)user);
            ctx.AddToAuditLog(log);
            if (!isValid)
            {
                log.ErrorMessage = errorMessage;
            }
            log.FieldName = fieldName;
            log.ItemTypeEnum = EnumItemType.StrokeValue;
            log.OldValue = "";
            log.NextValue = strokeValue.StringValue;
            log.Status = res;
            log.StrokeFieldValue = strokeValue;
            log.UploadLogReference.Value = uploadLog;
            log.RowNumber = rowNumber;

            UpdateAuditLog(ctx, log, strokeValue);
            return log;
        }

        /// <summary>
        /// Update Audit Log
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="log"></param>
        /// <param name="strokeValue"></param>
        /// <returns></returns>
        public static AuditLog UpdateAuditLog(EurekaStrokeEntities ctx, AuditLog log, StrokeFieldValue strokeValue)
        {
            strokeValue.UpdateAuditLog(ctx, log,null);
            return log;
        }

        /// <summary>
        /// Get old value for the entity
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetOldValue(EurekaStrokeEntities ctx, EntityKey key)
        {
            string val = "";
            val = ctx.GetOldValue(key);

            return val;

        }

        /// <summary>
        /// Create AuditLog entry
        /// </summary>
        /// <param name="res"></param>
        /// <param name="isValid"></param>
        /// <param name="uploadLog"></param>
        /// <param name="fieldName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static AuditLog CreateAuditLog(EurekaStrokeEntities ctx, int res, bool isValid, UploadLog uploadLog, string fieldName, string errorMessage, string value, int rowNumber)
        {
            object user = DataFacade.GetCurrentUser();
            AuditLog log = AuditLog.CreateAuditLog(0, (short)EnumActionType.Create, DateTime.Now, user == null ? 0 : (int)user);
            ctx.AddToAuditLog(log);
            if (!isValid)
            {
                log.ErrorMessage = errorMessage;
            }
            log.FieldName = fieldName;
            log.ItemTypeEnum = EnumItemType.StrokeValue;
            log.OldValue = "";
            log.NextValue = value;
            log.Status = res;
            log.UploadLogReference.Value = uploadLog;
            log.RowNumber = rowNumber;
            return log;
        }

        /// <summary>
        /// Create Audit Log for non-value changes
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="action"></param>
        /// <param name="type"></param>
        /// <param name="fieldName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static AuditLog CreateAuditLog(EurekaStrokeEntities ctx, EnumActionType action, EnumItemType type, string fieldName, string oldValue, string newValue)
        {
            object user = DataFacade.GetCurrentUser();
            AuditLog log = AuditLog.CreateAuditLog(0, (short)action, DateTime.Now, user == null ? 0 : (int)user);
            ctx.AddToAuditLog(log);
            
            log.FieldName = fieldName;
            log.ItemTypeEnum = type;
            log.OldValue = oldValue;
            log.NextValue = newValue;
            log.Status = 0;
            return log;
        }

        public static IEnumerable<AuditLog> GetAuditLogs(EurekaStrokeEntities ctx)
        {
            var items = from a in ctx.AuditLog select a;
            return items;
        }

    }
}
