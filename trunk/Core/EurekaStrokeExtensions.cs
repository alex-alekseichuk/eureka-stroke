using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Diagnostics;
using System.Data.Common;

namespace Eureka.Stroke
{
    /// <summary>
    /// Eureka Stroke object context
    /// </summary>
    public partial class EurekaStrokeEntities : global::System.Data.Objects.ObjectContext
    {
        partial void OnContextCreated()
        {
            this.SavingChanges += new EventHandler(OnSavingChanges);
        }

        private EnumActionType GetAction(EntityState state)
        {
            EnumActionType action = EnumActionType.Create;
            switch (state)
            {
                case EntityState.Added: 
                case EntityState.Detached:
                    action = EnumActionType.Create;
                    break;
                case EntityState.Modified:
                    action = EnumActionType.Update;
                    break;
                case EntityState.Deleted:
                    action = EnumActionType.Delete;
                    break;
            }

            return action;
        }

        private void Log(string name, EnumActionType action)
        {
            Logger.Instance.LogDebug("|---------------------|------------Saving entity:  " + action.ToString() + " " + name );
        }

        /// <summary>
        /// Get Old Value for StrokeValue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetOldValue(EntityKey key)
        {
            string val = "";
            // try to get old value
            ObjectStateEntry entry;
            bool ok = ObjectStateManager.TryGetObjectStateEntry(key, out entry);
            if (entry != null)
            {
                try
                {
                    val = entry.OriginalValues["StringValue"].ToString();
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogException("Error finding old value", ex);
                }
            }

            return val;

        }

        /// <summary>
        /// Handle audit trail activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSavingChanges(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<ObjectStateEntry> changes =
                    this.ObjectStateManager.GetObjectStateEntries(
                    EntityState.Added | EntityState.Deleted | EntityState.Modified);
                foreach (ObjectStateEntry stateEntryEntity in changes)
                {
                    if (stateEntryEntity.Entity != null)
                    {
                        Type t = stateEntryEntity.Entity.GetType();
                        EntityState state = stateEntryEntity.State;
                        string name = t.Name;
                        EnumActionType action = GetAction(state);
                        EnumItemType type = EnumItemType.StrokeValue;
                        //stateEntryEntity.
                        switch (name)
                        {
                            case "StrokeData":
                                StrokeData d = stateEntryEntity.Entity as StrokeData;
                                if (d != null)
                                {
                                    //Log(name, action);
                                }
                                break;

                            case "StrokeFieldValue":

                                StrokeFieldValue d2 = stateEntryEntity.Entity as StrokeFieldValue;
                                if (d2 != null)
                                {
                                    //Log(name, action);
                                }
                                break;

/*
                            case "FieldDefinition":

                                FieldDefinition d3 = stateEntryEntity.Entity as FieldDefinition;
                                if (d3 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldName;
                                    AddAuditRecord(action, type, stateEntryEntity, name, d3.Name);

                                }
                                break;
*/
                            case "EurekaField":
                                EurekaField d3 = stateEntryEntity.Entity as EurekaField;
                                if (d3 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldName;
                                    AddAuditRecord(action, type, stateEntryEntity, name, d3.Name);
                                }
                                break;
                            case "AlienField":
                                AlienField d4 = stateEntryEntity.Entity as AlienField;
                                if (d4 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldName;
                                    AddAuditRecord(action, type, stateEntryEntity, name, d4.Name);
                                }
                                break;
/*
                            case "FieldDefinitionMap":
                                FieldDefinitionMap d4 = stateEntryEntity.Entity as FieldDefinitionMap;
                                if (d4 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldMapping;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("mapping from {0} to {1}", d4.AlienFieldDefinition.Name, d4.EurekaFieldDefinition.Name);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);

                                }
                                break;
*/

/*
                            case "FieldValueDefinition":

                                FieldValueDefinition d5 = stateEntryEntity.Entity as FieldValueDefinition;
                                if (d5 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldValue;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("field Value {0} for field {1}", d5.FieldDefinition.Name, d5.Value);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);

                                }
                                break;
*/

                            case "EurekaValue":
                                EurekaValue d5 = stateEntryEntity.Entity as EurekaValue;
                                if (d5 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldValue;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("field Value {1} for field {0}", d5.EurekaField.Name, d5.Value);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);

                                }
                                break;
                            case "AlienValue":
                                AlienValue d6 = stateEntryEntity.Entity as AlienValue;
                                if (d6 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.FieldValue;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("field Value {1} for field {0}", d6.EurekaField.Name, d6.Value);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);
                                }
                                break;

                            case "Format":

                                Format d7 = stateEntryEntity.Entity as Format;
                                if (d7 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.Format;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("format {0}", d7.Name);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);

                                }
                                break;

                            case "Facility":

                                Format d8 = stateEntryEntity.Entity as Format;
                                if (d8 != null)
                                {
                                    //Log(name, action);
                                    type = EnumItemType.Format;
                                    string value = "";
                                    try
                                    {
                                        value = string.Format("Facility {0}", d8.Name);
                                    }
                                    catch { }
                                    AddAuditRecord(action, type, stateEntryEntity, name, value);

                                }
                                break;

                            default:
                                Log(name, action);
                                break;


                        }


                    }
                    else if (stateEntryEntity.IsRelationship)
                    {
                        EntityState state = stateEntryEntity.State;
                        //string name = stateEntryEntity.EntitySet.Name;
                        //Type t = stateEntryEntity.EntitySet.GetType();
                        //string name = t.Name;
                        try
                        {
                            EnumActionType action = GetAction(state);
                            if (stateEntryEntity.EntityKey != null)
                            {
                                var keys = stateEntryEntity.EntityKey.EntityKeyValues;
                                var entityName = stateEntryEntity.EntityKey.EntityContainerName;
                            }
                            //stateEntryEntity.EntitySet
                            //Log(name, action);
                        }
                        catch (Exception ex2)
                        {
                            Logger.Instance.LogActionException("Error when capturing audit log", ex2);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogActionException("Error when capturing audit log", ex);
            }
        }

        /// <summary>
        /// Add Audit record for av given type and action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="type"></param>
        /// <param name="entry"></param>
        /// <param name="name">Entity name</param>
        /// <param name="value">Value</param>
        private void AddAuditRecord(EnumActionType action, EnumItemType type, ObjectStateEntry entry, string name, string value)
        {
            AuditLog log;
            if (action == EnumActionType.Update)
            {
                log = CreateAuditLog(this, action, type, name, "", value);
                foreach (string propName in entry.GetModifiedProperties())
                {
                    EnumItemType typeField = GetItemType(propName, name);
                    string oldValue = "";
                    string newValue = "";
                    try
                    {
                        oldValue = entry.OriginalValues[propName].ToString();
                    }
                    catch { }
                    try
                    {
                        newValue = entry.CurrentValues[propName].ToString();
                    }
                    catch { }

                    log = CreateAuditLog(this, action, typeField, propName, oldValue, newValue);
                }
            }
            else if (action == EnumActionType.Create)
            {
                log = CreateAuditLog(this, action, type, name, "", value);
            }
            else if (action == EnumActionType.Delete)
            {
                log = CreateAuditLog(this, action, type, name, value, "");
            }
        }

        /// <summary>
        /// Get audit item type based on the column name and entity name
        /// </summary>
        /// <param name="property"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        private EnumItemType GetItemType(string property, string entityName)
        {
            EnumItemType type = EnumItemType.FieldName;
            switch (property)
            {
                case "Name": type = EnumItemType.FieldName; break;
                case "FieldType": type = EnumItemType.FieldType; break;
                case "Description": 
                    if (entityName == "FieldDefinition")
                        type = EnumItemType.FieldDescription; 
                    else
                        type = EnumItemType.FieldValueDescription; 
                        break;
                case "IsMandatory": type = EnumItemType.FieldMandatory; break;
                case "ValidationRegex": type = EnumItemType.FieldRegularExpression; break;
                case "RegularExpression": type = EnumItemType.FieldRegularExpression; break;
                case "FromFieldDefinitionID":
                case "ToFieldDefinitionID": 
                    type = EnumItemType.FieldMapping; break;
                case "Value":
                    type = EnumItemType.FieldValue; break;
                case "FromFieldValueDefinitionID":
                case "ToFieldValueDefinitionID":
                    type = EnumItemType.FieldValueMapping; break;


            }
            return type;
        }

        private static AuditLog CreateAuditLog(EurekaStrokeEntities ctx, EnumActionType action, EnumItemType type, string fieldName, string oldValue, string newValue)
        {
            object user = EurekaStrokeEntities.GetCurrentUser();
            AuditLog log = new AuditLog() { 
                Action = (short)action, 
                ActionDate = DateTime.Now, 
                ActionUser =  (user == null ? 0 : (int)user)
            };
            ctx.AddToAuditLog(log);

            log.FieldName = fieldName;
            log.ItemTypeEnum = type;
            log.OldValue = oldValue;
            log.NextValue = newValue;
            log.Status = 0;
            return log;
        }

        

        public static object GetCurrentUser()
        {

            return null;
        }

        /// <summary>
        /// Get affected entities of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="states"></param>
        /// <returns></returns>
        public IEnumerable<T> GetAffectedEntities<T>(EntityState states)
        {
            IEnumerable<ObjectStateEntry> changes = this.ObjectStateManager.GetObjectStateEntries(states);
            IEnumerable<T> items = (from c in changes where c.Entity != null select c.Entity).OfType<T>();

            return items;
        }

        /// <summary>
        /// Returns true if at least one entity of a given type T is affected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="states"></param>
        /// <returns></returns>
        public bool ExistAffectedEntity<T>(EntityState states)
        {
            IEnumerable<T> items = GetAffectedEntities<T>(states);
            T first = items.FirstOrDefault();
            return first!=null;
        }

        #region reports data sources

        /// <summary>
        /// Generate All Stroke Data
        /// </summary>
        /// <returns></returns>
        public StrokePivotData GetPivotStrokeData()
        {
            StrokePivotData data = null;
            try
            {

                //get the connection object from the DataContext
                //using (DbConnection conn = this.Connection)
                //{

                    //command text is of the format of 'ContainerName.Stored Proc Name' 
                    //string commandText = String.Format("{0}.{1}", this.DefaultContainerName, "PivotStrokeData");
                    string commandText = String.Format("{0}", "PivotStrokeData");
                    CommandType commType = CommandType.StoredProcedure;
                    DbCommand cmd = this.CreateStoreCommand(commandText, commType, null);
                    
                    if (this.Connection.State != System.Data.ConnectionState.Open)
                    {
                        this.Connection.Open();
                        this.CommandTimeout = 60 * 10;
                    }

                    var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                    var cnt = reader.FieldCount;

                    // header
                    IList<string> header = new List<string>(cnt);
                    for (int i = 0; i < cnt; i++)
                    {
                        string name = reader.GetName(i);
                        header.Add(name);
                    }


                    IList<IList<string>> rows = new List<IList<string>>();
                    while (reader.Read())
                    {
                        IList<string> rowVals = new List<string>(cnt);

                        for (int i = 0; i < cnt; i++)
                        {
                            object o = reader.GetValue(i);
                            string val = "";
                            if (o != null)
                                val = o.ToString();
                            rowVals.Add(val);
                        }
                        rows.Add(rowVals);
                    }
                    reader.Close();
                    data = new StrokePivotData(header, rows);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error download stroke data", ex);
            }

            return data;

        }



        /// <summary>
        /// Generate Data for specified Facility
        /// </summary>
        /// <returns></returns>
        public StrokePivotData GetPivotFacilityData(int facilityID)
        {
            StrokePivotData data = null;
            try
            {
                string commandText = String.Format("{0}", "PivotFacilityData");
                CommandType commType = CommandType.StoredProcedure;
                DbCommand cmd = this.CreateStoreCommand(commandText, commType, null);

                var param = cmd.CreateParameter();
                param.ParameterName = "@facilityID";
                param.Value = facilityID;
                cmd.Parameters.Add(param);

                if (this.Connection.State != System.Data.ConnectionState.Open)
                {
                    this.Connection.Open();
                    this.CommandTimeout = 60 * 10;
                }

                var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                var cnt = reader.FieldCount;

                // header
                IList<string> header = new List<string>(cnt);
                for (int i = 0; i < cnt; i++)
                {
                    string name = reader.GetName(i);
                    header.Add(name);
                }


                IList<IList<string>> rows = new List<IList<string>>();
                while (reader.Read())
                {
                    IList<string> rowVals = new List<string>(cnt);

                    for (int i = 0; i < cnt; i++)
                    {
                        object o = reader.GetValue(i);
                        string val = "";
                        if (o != null)
                            val = o.ToString();
                        rowVals.Add(val);
                    }
                    rows.Add(rowVals);
                }
                reader.Close();
                data = new StrokePivotData(header, rows);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error download stroke data", ex);
            }

            return data;

        }

        /// <summary>
        /// Generate Data for specified custom report.
        /// It means a Data Source.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="reportingSource">Reporting Source key or empty string for all data available for user</param>
        /// <returns></returns>
        public DataSet GetPivotReportData(int userID, string reportingSource)
        {
            var ds = new DataSet();
            try
            {
                string commandText = String.Format("{0}", "PivotReportData");
                CommandType commType = CommandType.StoredProcedure;
                DbCommand cmd = this.CreateStoreCommand(commandText, commType, null);

                var param = cmd.CreateParameter();
                param.ParameterName = "@userID";
                param.Value = userID;
                cmd.Parameters.Add(param);

                param = cmd.CreateParameter();
                param.ParameterName = "@reportingSource";
                param.Value = reportingSource;
                cmd.Parameters.Add(param);

                var maxrows = Config.GetConfigValue("StrokeDataReport.MaxRows", "2000").ToInt();
                param = cmd.CreateParameter();
                param.ParameterName = "@maxrows";
                param.Value = maxrows;
                cmd.Parameters.Add(param);

                if (this.Connection.State != System.Data.ConnectionState.Open)
                {
                    this.Connection.Open();
                    this.CommandTimeout = 60 * 10;
                }
                
                var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                ds.Load(reader, LoadOption.OverwriteChanges, "StrokeData");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error download stroke data", ex);
            }

            return ds;
        }

        /// <summary>
        /// Generate Data for specified Facilities list
        /// </summary>
        /// <param name="facilitiesIDs">coma-separeted list of facilities ID</param>
        /// <returns></returns>
        public DataSet GetPivotFacilitiesData(string facilitiesIDs)
        {
            var ds = new DataSet();
            try
            {
                string commandText = String.Format("{0}", "PivotFacilitiesData");
                CommandType commType = CommandType.StoredProcedure;
                DbCommand cmd = this.CreateStoreCommand(commandText, commType, null);

                var param = cmd.CreateParameter();
                param.ParameterName = "@facilitiesIDs";
                param.Value = facilitiesIDs;
                cmd.Parameters.Add(param);

                var maxrows = Config.GetConfigValue("StrokeDataReport.MaxRows", "2000").ToInt();
                param = cmd.CreateParameter();
                param.ParameterName = "@maxrows";
                param.Value = maxrows;
                cmd.Parameters.Add(param);

                if (this.Connection.State != System.Data.ConnectionState.Open)
                {
                    this.Connection.Open();
                    this.CommandTimeout = 60 * 10;
                }

                var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                ds.Load(reader, LoadOption.OverwriteChanges, "StrokeData");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error download stroke data", ex);
            }

            return ds;
        }
        #endregion

        public int GetEurekaFieldIndex(int id)
        {
            var ret = 0;
            try
            {
                string commandText = String.Format("{0}", "EurekaField_GetIndex");
                CommandType commType = CommandType.StoredProcedure;


                using (DbCommand cmd = this.CreateStoreCommand(commandText, commType, null))
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = "@ID";
                    param.Value = id;
                    cmd.Parameters.Add(param);

                    if (this.Connection.State != System.Data.ConnectionState.Open)
                    {
                        this.Connection.Open();
                        this.CommandTimeout = 60 * 10;
                    }

                    using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                            ret = reader.GetInt32(0);
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error in GetEurekaFieldIndex", ex);
            }

            return ret;

        }

        /// <summary>
        /// NonQuery method to re-sort all custom reports folders by name
        /// </summary>
        /// <returns></returns>
        public int SortFoldersByName()
        {
            var ret = 0;
            try
            {
                var commandText = String.Format("{0}", "SortFolders");
                var commType = CommandType.StoredProcedure;

                using (var cmd = this.CreateStoreCommand(commandText, commType, null))
                {
                    if (this.Connection.State != System.Data.ConnectionState.Open)
                    {
                        this.Connection.Open();
                        this.CommandTimeout = 60 * 10;
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Logger.Instance.LogActionException("Error in SortFoldersByName", ex);
            }

            return ret;

        }


    }


    /// <summary>
    /// UploadLog extensions
    /// </summary>
    public partial class UploadLog : global::System.Data.Objects.DataClasses.EntityObject
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            this.FacilityReference.EnsureLoad();
            this.FormatReference.EnsureLoad();
            string facilityName = Facility != null ? Facility.Name : "None";
            string formatName = Format != null ? Format.Name : "None";

            DateTime date = this.StartedDate ?? DateTime.Now;
            sb.AppendFormat("({0}) {1}_{2}_{3}-{4}", ID, OriginalFilename ?? "", date, facilityName, formatName);
            return sb.ToString();
        }

        public string FriendlyName
        {
            get
            {
                return ToString();
            }
        }
    }


    /// <summary>
    /// Eureka Field
    /// </summary>
    public partial class EurekaField : global::System.Data.Objects.DataClasses.EntityObject
    {
        public EnumFieldType FieldTypeEnum
        {
            get { return (EnumFieldType)FieldType; }
            set { FieldType = (short)value; }
        }

        /// <summary>
        /// Returns true if this is a native (eureka) field
        /// </summary>
        public bool IsNative
        {
            get
            {
                bool isNative = true;
                this.FormatReference.EnsureLoad();
                if (Format != null)
                {
                    isNative = Format.IsNative ?? true;
                }
                return isNative;
            }
        }

        /// <summary>
        /// Returns true if this is alien field
        /// </summary>
        public bool IsAlien
        {
            get
            {
                bool isNative = IsNative;
                return !isNative;
            }
        }

    }

    /// <summary>
    /// Audit Log
    /// </summary>
    public partial class AuditLog : global::System.Data.Objects.DataClasses.EntityObject
    {
        public EnumItemType ItemTypeEnum
        {
            get { return (EnumItemType)ItemType; }
            set { ItemType = (short)value; }
        }

        public EnumActionType ActionEnum
        {
            get { return (EnumActionType)Action; }
            set { Action = (short)value; }
        }

        /// <summary>
        /// Create Audit Log entry
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
            object user = EurekaStrokeEntities.GetCurrentUser();
            AuditLog log = AuditLog.CreateAuditLog(0, (short)action, DateTime.Now, user == null ? 0 : (int)user);
            ctx.AddToAuditLog(log);

            log.FieldName = fieldName;
            log.ItemTypeEnum = type;
            log.OldValue = oldValue;
            log.NextValue = newValue;
            log.Status = 0;
            return log;
        }


    }

    public partial class StrokeFieldValue : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a copy of the entity
        /// </summary>
        /// <returns></returns>
        public StrokeFieldValue Clone(EurekaStrokeEntities ctx, EnumActionType? type)
        {
            StrokeFieldValue to = this.CloneEntity() as StrokeFieldValue;
            if (type != null && type.Value != EnumActionType.Create)
                CopyAudit(ctx, to, type);
            else
                RemoveAudit(ctx);
            return to;
        }

        /// <summary>
        /// Copy audit data
        /// </summary>
        /// <param name="to"></param>
        public void CopyAudit(EurekaStrokeEntities ctx, StrokeFieldValue to, EnumActionType? type)
        {
            if (this.AuditLog != null)
            {
                if (this.AuditLog.Count > 0)
                {
                    List<AuditLog> logs = this.AuditLog.ToList();
                    foreach (var log in logs)
                    {
                        if (log != null)
                        {
                            AuditLog.Remove(log);
                            if (type != null && type.Value != EnumActionType.Create)
                            {
                                to.UpdateAuditLog(ctx, log, type);
                                to.AuditLog.Add(log);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Remove audit data
        /// </summary>
        /// <param name="ctx"></param>
        public void RemoveAudit(EurekaStrokeEntities ctx)
        {
            if (this.AuditLog != null)
            {
                if (this.AuditLog.Count > 0)
                {
                    List<AuditLog> logs = this.AuditLog.ToList();
                    foreach (var log in logs)
                    {
                        if (log != null)
                        {
                            AuditLog.Remove(log);
                            ctx.Detach(log);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update audit log action type and old value if applicable
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="log"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public AuditLog UpdateAuditLog(EurekaStrokeEntities ctx, AuditLog log, EnumActionType? type)
        {
            if (EntityState == EntityState.Deleted ||
                EntityState == EntityState.Detached)
            {
                log.Action = (short)EnumActionType.Delete;
            }
            else if (EntityState == EntityState.Modified)
            {
                log.Action = (short)EnumActionType.Update;
                // try to get old value
                log.OldValue = ctx.GetOldValue(EntityKey);
            }
            else if (type != null)
            {
                log.Action = (short)type.Value;
                // try to get old value
                log.OldValue = ctx.GetOldValue(EntityKey);
            }
            return log;
        }

        /// <summary>
        /// copy data to another entity
        /// </summary>
        /// <param name="to"></param>
        public void CopyTo(EurekaStrokeEntities ctx, StrokeFieldValue to)
        {
            if (this.BooleanValue.HasValue)
                to.BooleanValue = this.BooleanValue;
            if (this.IntegerValue.HasValue)
                to.IntegerValue = this.IntegerValue;
            if (this.DecimalValue.HasValue)
                to.DecimalValue = this.DecimalValue;
            if (this.DateTimeValue.HasValue)
                to.DateTimeValue = this.DateTimeValue;
            to.StringValue = this.StringValue;
            to.RawValue = this.RawValue;
            to.Status = this.Status;
            CopyAudit(ctx, to,null);
        }

        public override bool Equals(object obj)
        {
            if (obj is StrokeFieldValue)
            {
                StrokeFieldValue x = obj as StrokeFieldValue;
                return (x.StringValue == StringValue);
            }
            else return base.Equals(obj);
        }
    }

    public partial class Facility : global::System.Data.Objects.DataClasses.EntityObject
    {
        private DateTime? maxAdmissionDate = null;

        /// <summary>
        /// Get max admission date per Facility
        /// </summary>
        public DateTime MaxAdmissionDate
        {
            get
            {
                if (maxAdmissionDate != null) 
                    return maxAdmissionDate.Value;
                this.UploadLog.EnsureLoad();

                try
                {
                    //var strokeData = from fu in this.UploadLog
                    //                 from s in fu.StrokeData
                    //                 from su in s.UploadLog
                    //                 //from v in s.StrokeFieldValue
                    //                 where fu.Facility.ID == this.ID
                    //                 && su.Facility.ID == this.ID
                    //                 select s;
                    EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
           

                    var strokeData = from fu in ctx.UploadLog.Include("StrokeData")
                                     from s in fu.StrokeData
                                     where 
                                     fu.Facility.ID == this.ID
                                     //&& fu.ID == this.UploadLog.ID
                                     select s;

                    var cnt = strokeData.Count();

                    if (cnt > 0)
                    {
                        maxAdmissionDate = strokeData.Max(d => d.AdmissionDate);
                    }
                    //var data = strokeData.ToList();
                } catch(Exception ex) {
                    Logger.Instance.LogActionException("Error getting MaxAdmissionDate", ex);
                }
                if (maxAdmissionDate != null) return maxAdmissionDate.Value;
                else maxAdmissionDate = Utils.GetDateMinValue();
                return maxAdmissionDate.Value;
                
            }
        }

        public void ResetMaxAdmissionDate()
        {
            maxAdmissionDate = null;
        }
    }

    /// <summary>
    /// Field value comparer
    /// </summary>
    public class StrokeFieldValueComparer : IEqualityComparer<StrokeFieldValue> 
    {

        #region IEqualityComparer<StrokeFieldValue> Members

        public bool Equals(StrokeFieldValue x, StrokeFieldValue y)
        {
            return (x.StringValue == y.StringValue);
        }

        public int GetHashCode(StrokeFieldValue obj)
        {
            return obj.StringValue.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// EurekaField equality comparer
    /// </summary>
    public class EurekaFieldComparer : IEqualityComparer<EurekaField>
    {

        #region IEqualityComparer<FieldDefinition> Members

        public bool Equals(EurekaField x, EurekaField y)
        {
            return (x.Name == y.Name);
        }

        public int GetHashCode(EurekaField obj)
        {
            return obj.Name.GetHashCode();
        }

        #endregion
    }
    public class AlienFieldComparer : IEqualityComparer<AlienField>
    {

        #region IEqualityComparer<FieldDefinition> Members

        public bool Equals(AlienField x, AlienField y)
        {
            return (x.Name == y.Name);
        }

        public int GetHashCode(AlienField obj)
        {
            return obj.Name.GetHashCode();
        }

        #endregion
    }
    

    public static class Helper
    {
        /// <summary>
        /// Attach an EntityObject that was modified when detached
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objectDetached">DetachedObject</param>
        public static void AttachUpdated(this ObjectContext obj, EntityObject objectDetached)
        {
            if (objectDetached.EntityState == EntityState.Detached)
            {
                object original = null;
                if (obj.TryGetObjectByKey(objectDetached.EntityKey, out original))
                    obj.ApplyPropertyChanges(objectDetached.EntityKey.EntitySetName, objectDetached);
                else
                    throw new ObjectNotFoundException();
            }
        }
    }

    public class StrokePivotData
    {
        public IList<string> Header { get; set; }
        public IList<IList<string>> Rows { get; set; }

        public StrokePivotData(IList<string> header, IList<IList<string>> rows)
        {
            Header = header;
            Rows = rows;
        }
    }

}
