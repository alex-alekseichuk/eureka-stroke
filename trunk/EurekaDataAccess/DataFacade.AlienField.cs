using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Diagnostics;

namespace Eureka.Stroke.Data
{
    /// <summary>
    /// Data access routines for field/values definitions manipulations
    /// </summary>
    public static partial class DataFacade
    {
        /// <summary>
        /// Create a default field defintion
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static AlienField GetDefaultAlienFieldDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            AlienField def = AlienField.CreateAlienField(0, fieldName);
            ctx.AddToAlienField(def);
            def.FormatReference.Value = format;
            return def;
        }

        /// <summary>
        /// Get Alien field definition by field name
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fieldName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static AlienField GetAlienFieldDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            var item = (from f in ctx.AlienField.Include("EurekaField").Include("EurekaField.AlienValue").Include("EurekaField.EurekaValue").Include("Format")
                        where f.Name == fieldName
                        && f.Format.ID == format.ID
                        select f).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Get alien field by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AlienField GetAlienFieldDefinitionByID(EurekaStrokeEntities ctx, int id)
        {
            var item = (from f in ctx.AlienField.Include("EurekaField").Include("EurekaField.AlienValue").Include("EurekaField.EurekaValue").Include("Format")
                        where f.ID == id
                        select f).FirstOrDefault();
            return item;
        }

        public static IList<AlienField> GetAlienFieldDefinitions(EurekaStrokeEntities ctx, Format format)
        {
            var list = GetAlienFieldDefinitions(ctx, format, false);
            return list;
        }


        /// <summary>
        /// Get a list of all field definitions for a given format
        /// ordered by field id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IList<AlienField> GetAlienFieldDefinitions(EurekaStrokeEntities ctx, Format format, bool removeDuplicates)
        {
            var items = (from field in ctx.AlienField.Include("EurekaField").Include("EurekaField.AlienValue").Include("EurekaField.EurekaValue").Include("Format")
                         where field.Format.ID == format.ID
                         orderby field.ID
                         select field).ToList();
            if (removeDuplicates)
                items = items.Distinct(new AlienFieldComparer()).ToList();
            return items;
        }

        public static IList<AlienField> GetAlienFieldDefinitionsForEurekaField(EurekaStrokeEntities ctx, EurekaField eurekaField, Format format)
        {
            var items = (from field in ctx.AlienField.Include("EurekaField").Include("EurekaField.AlienValue").Include("EurekaField.EurekaValue").Include("Format")
                         where field.Format.ID == format.ID && field.EurekaField.ID == eurekaField.ID
                         orderby field.ID
                         select field).ToList();
            return items;
        }

        /// <summary>
        /// Get a list of all field value definitions for a given field
        /// ordered by value id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IList<AlienValue> GetAlienValues(EurekaStrokeEntities ctx, AlienField field)
        {
            List<AlienValue> list = new List<AlienValue>();
            field.EurekaFieldReference.EnsureLoad();
            EurekaField eurekaField = field.EurekaField;
            Format format = field.Format;

            if (eurekaField!=null && format!=null)
                list = (from value in ctx.AlienValue
                    where value.EurekaField.ID == eurekaField.ID
                    && value.Format.ID == format.ID
                    orderby value.ID
                    select value).ToList();
            return list;
        }

        public static IList<AlienValue> GetEurekaAlienValues(EurekaStrokeEntities ctx, EurekaField eurekaField, Format format)
        {
            List<AlienValue> list = null;
            list = (from value in ctx.AlienValue.Include("EurekaField").Include("EurekaValue")
                    where value.EurekaField.ID == eurekaField.ID && value.Format.ID == format.ID && null != value.EurekaValue
                    orderby value.EurekaValue.ID
                    select value).ToList();
            return list;
        }

        public static IList<AlienValue> GetUnmappedAlienValues(EurekaStrokeEntities ctx, EurekaField eurekaField, Format format)
        {
            List<AlienValue> list = null;
            list = (from value in ctx.AlienValue.Include("EurekaField").Include("EurekaValue")
                    where value.EurekaField.ID == eurekaField.ID && value.Format.ID == format.ID && null == value.EurekaValue
                    orderby value.ID
                    select value).ToList();
            return list;
        }


        /// <summary>
        /// Simple getting value by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="valueID"></param>
        /// <returns></returns>
        public static AlienValue GetAlienFieldValueByID(EurekaStrokeEntities ctx, int valueID)
        {
            return (from value in ctx.AlienValue
                    where value.ID == valueID
                    select value).FirstOrDefault();
        }

        /// <summary>
        /// Simple getting value by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static AlienValue GetAlienFieldValueByValue(EurekaStrokeEntities ctx, string value, int eurekaFieldID, int alienFormatID)
        {
            return (from item in ctx.AlienValue.Include("EurekaField").Include("Format")
                    where item.Value == value && item.EurekaField.ID == eurekaFieldID && item.Format.ID == alienFormatID
                    select item).FirstOrDefault();
        }

        /// <summary>
        /// Get specified alien value by parent eureka value and specified alien field
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaValueID"></param>
        /// <param name="alienFieldID"></param>
        /// <returns></returns>
        public static AlienValue GetAlienFieldValueDefinitionByParents(EurekaStrokeEntities ctx, int eurekaValueID, int alienFieldID)
        {
            AlienField field = GetAlienFieldDefinitionByID(ctx, alienFieldID);
            Format format = field.Format;

            AlienValue val = null;

            val = (from v in ctx.AlienValue.Include("EurekaValue").Include("EurekaField").Include("Format")
                    where v.EurekaValue.ID == eurekaValueID
                    && v.Format.ID == format.ID
                    select v).FirstOrDefault();
            return val;
        }

        private static List<EntityObject> GetAlienFieldObjectsToRemove(AlienField field)
        {

            var itemsToRemove = new List<EntityObject>();

            // delete the field
            itemsToRemove.Add(field);

            return itemsToRemove;
        }

        /// <summary>
        /// Try to delete a field definition
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <param name="msg">error message</param>
        /// <returns>error code or 0 if it's ok</returns>
        public static int TryDeleteAlienFieldDefinition(EurekaStrokeEntities ctx, int id, out string msg)
        {
            msg = string.Empty;
            AlienField field = null;

            try
            {
                field = GetAlienFieldDefinitionByID(ctx, id);
                if (null == field)
                {
                    msg = "The field with ID = " + id + " does not exist";
                    return 1;
                }

                // get a list of entities for removal
                var itemsToRemove = GetAlienFieldObjectsToRemove(field);
                foreach (var item in itemsToRemove)
                {
                    ctx.DeleteObject(item);
                }

                ctx.SaveChanges(true);
            }
            catch (Exception ex)
            {
                msg = string.Format("Error deleting field '{0}'", field.Name);
                Logger.Instance.LogException(msg, ex);
                return 3;
            }
            return 0;
        }

        /// <summary>
        /// Try to delete a value definition
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id">id of the value</param>
        /// <param name="messageOut"></param>
        /// <returns>error code or 0 if it's ok</returns>
        public static int TryDeleteAlienValue(EurekaStrokeEntities ctx, int id, out string messageOut)
        {
            messageOut = string.Empty;

            var value = GetAlienFieldValueByID(ctx, id);
            if (null == value)
            {
                messageOut = "The value with ID = " + id + " does not exist";
                return 1;
            }


            try
            {
                ctx.DeleteObject(value);
                ctx.SaveChanges(true);
            }
            catch (Exception ex)
            {
                messageOut = string.Format("Error deleting value '{0}'", value.Value);
                Logger.Instance.LogException(messageOut, ex);
                return 3;
            }
            return 0;
        }


        public static bool SaveAlienFieldDefinition(EurekaStrokeEntities ctx, AlienField alienField)
        {
            try
            {
                if (0 == alienField.ID)
                    ctx.AddToAlienField(alienField);
                else
                    ctx.AttachUpdated(alienField);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException("Can't save Alien Field", ex);
                return false;
            }
            return true;
        }


        public static bool SaveAlienValueDefinition(EurekaStrokeEntities ctx, AlienValue alienValue)
        {
            try
            {
                if (0 == alienValue.ID)
                    ctx.AddToAlienValue(alienValue);
                else
                    ctx.AttachUpdated(alienValue);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException("Can't save Alien Value", ex);
                return false;
            }
            return true;
        }

    }
    /// <summary>
    /// Storage for alien fields definitions
    /// </summary>
    public class AlienFieldDefinitionCache
    {

        protected Dictionary<string, AlienField> map = null;
        protected Format format = null;

        [ThreadStatic]
        public static AlienFieldDefinitionCache instance = null;

        protected static object locker = new object();

        public static AlienFieldDefinitionCache Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new AlienFieldDefinitionCache();
                }
                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        public Dictionary<string, AlienField> Load(Format format, EurekaStrokeEntities ctx)
        {
            this.format = format;
            var fields = DataFacade.GetAlienFieldDefinitions(ctx, format, true);
            if (fields != null)
            {
                map = fields.ToDictionary<AlienField, string, AlienField>(f => f.Name.Trim(), f => f);
            }
            return map;
        }

        public AlienField this[Format format, string fieldName]
        {
            get
            {
                AlienField definition = null;
                if (this.format == null || format.ID != this.format.ID)
                {
                    EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
                    Load(format, ctx);
                }
                if (map != null)
                {
                    if (map.ContainsKey(fieldName.Trim()))
                        definition = map[fieldName.Trim()];
                }
                return definition;
            }
        }

    }



}
