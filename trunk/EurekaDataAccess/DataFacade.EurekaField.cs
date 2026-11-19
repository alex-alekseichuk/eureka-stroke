using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

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
        public static EurekaField GetDefaultEurekaFieldDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            EurekaField def = EurekaField.CreateEurekaField(0, fieldName, (short)EnumFieldType.String, true, true);
            ctx.AddToEurekaField(def);
            def.FormatReference.Value = format;
            return def;
        }

        /// <summary>
        /// Get eureka field definition by field name
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fieldName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static EurekaField GetEurekaFieldDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            var item = (from f in ctx.EurekaField.Include("AlienField").Include("AlienValue").Include("EurekaValue").Include("KeyFormat")
                        where f.Name == fieldName
                        && f.Format.ID == format.ID
                        select f).FirstOrDefault();
            return item;
        }

        /// <summary>
        /// Get eureka field by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EurekaField GetEurekaFieldDefinitionByID(EurekaStrokeEntities ctx, int id)
        {
            var item = (from f in ctx.EurekaField.Include("AlienField").Include("AlienValue").Include("EurekaValue").Include("KeyFormat")
                        where f.ID == id
                        select f).FirstOrDefault();
            return item;
        }

        public static IList<EurekaField> GetEurekaFieldDefinitions(EurekaStrokeEntities ctx, Format format)
        {
            var list = GetEurekaFieldDefinitions(ctx, format, false);
            return list;
        }


        /// <summary>
        /// Get a list of all field definitions for a given format
        /// ordered by field id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IList<EurekaField> GetEurekaFieldDefinitions(EurekaStrokeEntities ctx, Format format, bool removeDuplicates)
        {
            List<EurekaField> items = (from field in ctx.EurekaField.Include("AlienField").Include("AlienValue").Include("EurekaValue").Include("KeyFormat")
                         where field.Format.ID == format.ID
                         orderby field.Name
                         select field).ToList();
            if (removeDuplicates)
                items = items.Distinct(new EurekaFieldComparer()).ToList();
            return items;
        }


        /// <summary>
        /// Return a list of objects to remove for specified field (eureka or alien)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static List<EntityObject> GetEurekaFieldObjectsToRemove(EurekaField field)
        {
            
            var itemsToRemove = new List<EntityObject>();

            // delete values 
            field.AlienValue.EnsureLoad();
            field.AlienValue.ForEach(itemsToRemove.Add);

            // delete values 
            field.EurekaValue.EnsureLoad();
            field.EurekaValue.ForEach(itemsToRemove.Add);
            
            // delete linked alien fields
            var alienFields = field.AlienField;

            alienFields.ForEach(f =>
            {
                // recursive call to retrieve objects for removal
                var list = GetAlienFieldObjectsToRemove(f);
                itemsToRemove.AddRange(list);
            }
            );
            

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
        public static int TryDeleteEurekaFieldDefinition(EurekaStrokeEntities ctx, int id, out string msg)
        {
            msg = string.Empty;
            EurekaField field = null;

            try
            {
                field = GetEurekaFieldDefinitionByID(ctx, id);
                if (null == field)
                {
                    msg = "The field with ID = " + id + " does not exist";
                    return 1;
                }

                bool isEureka = field.IsNative;
                if (isEureka)
                {
                    field.StrokeFieldValue.EnsureLoad();
                    if (field.StrokeFieldValue.Count > 0)
                    {
                        msg = "The field '" + field.Name + "' cannot be deleted because it already has data";
                        return 2;
                    }

                }

                // get a list of entities for removal
                var itemsToRemove = GetEurekaFieldObjectsToRemove(field);
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




        public static bool SaveEurekaFieldDefinition(EurekaStrokeEntities ctx, EurekaField eurekaField)
        {
            try
            {
                if (0 == eurekaField.ID)
                    ctx.AddToEurekaField(eurekaField);
                else
                    ctx.AttachUpdated(eurekaField);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException("Can't save Eureka Field", ex);
                return false;
            }
            return true;
        }

        /*
        /// <summary>
        /// Calcualte index of the EurekaField in the sorted by Name list
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaField"></param>
        /// <returns>The index</returns>
        public static int GetEurekaFieldIndex(EurekaStrokeEntities ctx, EurekaField eurekaField)
        {
            var rowIndex = ctx.EurekaField
                .Select((field, index) => new { Index = index, field.ID })
                .Where(record => record.ID == eurekaField.ID).FirstOrDefault().Index;
            return rowIndex;
        }
        */

        /// <summary>
        /// Get a list of all field value definitions for a given field
        /// ordered by value id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IList<EurekaValue> GetEurekaValues(EurekaStrokeEntities ctx, EurekaField field)
        {
            List<EurekaValue> list = null;
            list = (from value in ctx.EurekaValue.Include("EurekaField")
                    where value.EurekaField.ID == field.ID
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
        public static EurekaValue GetEurekaFieldValueByID(EurekaStrokeEntities ctx, int valueID)
        {
            return (from value in ctx.EurekaValue
                    where value.ID == valueID
                    select value).FirstOrDefault();
        }

        /// <summary>
        /// Simple getting value by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="valueID"></param>
        /// <returns></returns>
        public static EurekaValue GetEurekaFieldValueByValue(EurekaStrokeEntities ctx, string value, int eurekaFieldID)
        {
            return (from item in ctx.EurekaValue.Include("EurekaField")
                    where item.Value == value && item.EurekaField.ID == eurekaFieldID
                    select item).FirstOrDefault();
        }


        public static bool SaveEurekaValueDefinition(EurekaStrokeEntities ctx, EurekaValue eurekaValue)
        {

            try
            {
                if (0 == eurekaValue.ID)
                    ctx.AddToEurekaValue(eurekaValue);
                else
                    ctx.AttachUpdated(eurekaValue);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException("Can't save Eureka Value", ex);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Try to delete a eureka value by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="id">id of the value</param>
        /// <param name="messageOut"></param>
        /// <returns>error code or 0 if it's ok</returns>
        public static int TryDeleteEurekaValue(EurekaStrokeEntities ctx, int id, out string messageOut)
        {
            messageOut = string.Empty;

            var value = GetEurekaFieldValueByID(ctx, id);
            if (null == value)
            {
                messageOut = "The value with ID = " + id + " does not exist";
                return 1;
            }

            string message;
            var ret = TryDeleteEurekaAlienValues(ctx, value, out message);
            if (0 != ret)
            {
                messageOut = message;
                return ret;
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

        /// <summary>
        /// Try to delete all alien values for specified eureka value
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaValue">eureka Value</param>
        /// <param name="msg"></param>
        /// <returns>error code or 0 if it's ok</returns>
        public static int TryDeleteEurekaAlienValues(EurekaStrokeEntities ctx, EurekaValue eurekaValue, out string msg)
        {
            msg = string.Empty;

            try
            {
                var itemsToRemove = new List<EntityObject>();
                eurekaValue.AlienValue.EnsureLoad();
                eurekaValue.AlienValue.ForEach(itemsToRemove.Add);
                foreach (var item in itemsToRemove)
                {
                    ctx.DeleteObject(item);
                }
                //eurekaValue.AlienValue.ForEach(ctx.DeleteObject);
                ctx.SaveChanges(true);
            }
            catch (Exception ex)
            {
                msg = string.Format("Error deleting alien values of eureka value '{0}'", eurekaValue.Value);
                Logger.Instance.LogException(msg, ex);
                return 3;
            }
            return 0;
        }

        


    }


}
