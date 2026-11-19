using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Eureka.Stroke.Data
{
    /// <summary>
    /// Data access routines for field definitions manipulations
    /// </summary>
    public static partial class DataFacade
    {
        /// <summary>
        /// Create a default field defintion
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static FieldDefinition GetDefaultDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            FieldDefinition def = FieldDefinition.CreateFieldDefinition(0, fieldName, (short)EnumFieldType.String, false, true);
            ctx.AddToFieldDefinition(def);
            def.FormatReference.Value = format;
            return def;
        }

        /// <summary>
        /// Get alien field definition by field name
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fieldName"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FieldDefinition GetFieldDefinition(EurekaStrokeEntities ctx, string fieldName, Format format)
        {
            var item = (from f in ctx.FieldDefinition.Include("AlienFieldDefinitionMaps").Include("AlienFieldDefinitionMaps.EurekaFieldDefinition")
                        where f.Name == fieldName
                        && f.Format.ID == format.ID
                        select f).FirstOrDefault();
            return item;
        }

        public static FieldDefinition GetFieldDefinitionByID(EurekaStrokeEntities ctx, int id)
        {
            var item = (from f in ctx.FieldDefinition
                        where f.ID == id
                        select f).FirstOrDefault();
            return item;
        }

        public static IList<FieldDefinition> GetFieldDefinitions(EurekaStrokeEntities ctx, Format format)
        {
            var list = GetFieldDefinitions(ctx, format, false);
            return list;
        }


        /// <summary>
        /// Get a list of all field definitions for a given format
        /// ordered by field id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static IList<FieldDefinition> GetFieldDefinitions(EurekaStrokeEntities ctx, Format format, bool removeDuplicates)
        {
            var items = (from field in ctx.FieldDefinition
                         where field.Format.ID == format.ID
                         orderby field.ID
                         select field).ToList();
            if (removeDuplicates)
                items = items.Distinct(new FieldDefinitionComparer()).ToList();
            return items;
        }

        /// <summary>
        /// Get all fields maps from alienFormat to eurekaFormat
        /// ordered by eureka field id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaFormat"></param>
        /// <param name="alienFormat"></param>
        /// <returns></returns>
        public static IList<FieldDefinitionMap> GetFieldsMaps(EurekaStrokeEntities ctx, Format eurekaFormat, Format alienFormat)
        {
            return
                (from map in ctx.FieldDefinitionMap.Include("AlienFieldDefinition").Include("EurekaFieldDefinition")
                 where map.EurekaFieldDefinition.Format.ID == eurekaFormat.ID
                       && map.AlienFieldDefinition.Format.ID == alienFormat.ID
                 orderby map.EurekaFieldDefinition.ID, map.AlienFieldDefinition.ID
                 select map).ToList();
        }

        /// <summary>
        /// Get all fields maps from alienFormat to eurekaFormat
        /// for specified eureka field
        /// ordered by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaField"></param>
        /// <param name="alienFormat"></param>
        /// <returns></returns>
        public static IList<FieldDefinitionMap> GetFieldMaps(EurekaStrokeEntities ctx, FieldDefinition eurekaField, Format alienFormat)
        {
            return
                (from map in ctx.FieldDefinitionMap.Include("AlienFieldDefinition").Include("EurekaFieldDefinition")
                 where map.EurekaFieldDefinition.ID == eurekaField.ID
                       && map.AlienFieldDefinition.Format.ID == alienFormat.ID
                 orderby map.AlienFieldDefinition.ID
                 select map).ToList();
        }

        /// <summary>
        /// Get mapping for specified alien field
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="alienFieldID"></param>
        /// <returns></returns>
        public static FieldDefinitionMap GetFieldMapByAlienFieldID(EurekaStrokeEntities ctx, int alienFieldID)
        {
            return
                (from map in ctx.FieldDefinitionMap.Include("AlienFieldDefinition")
                 where map.AlienFieldDefinition.ID == alienFieldID
                 select map).FirstOrDefault();
        }


        /// <summary>
        /// Get a list of all field value definitions for a given field
        /// ordered by value id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static IList<FieldValueDefinition> GetValueDefinitions(EurekaStrokeEntities ctx, FieldDefinition field)
        {
            return (from value in ctx.FieldValueDefinition
                    where value.FieldDefinition.ID == field.ID
                    orderby value.ID
                    select value).ToList();
        }

        /// <summary>
        /// Simple getting value by ID
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="valueID"></param>
        /// <returns></returns>
        public static FieldValueDefinition GetFieldValueDefinitionByID(EurekaStrokeEntities ctx, int valueID)
        {
            return (from value in ctx.FieldValueDefinition
                    where value.ID == valueID
                    select value).FirstOrDefault();
        }

        /// <summary>
        /// Get specified alien value by parent eureka value and specified alien field
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaValueID"></param>
        /// <param name="alienFieldID"></param>
        /// <returns></returns>
        public static FieldValueDefinition GetFieldValueDefinitionByParents(EurekaStrokeEntities ctx, int eurekaValueID, int alienFieldID)
        {
            return (from map in ctx.FieldValueDefinitionMap.Include("EurekaFieldValueDefinition").Include("AlienFieldValueDefinition").Include("AlienFieldValueDefinition.FieldDefinition")
                    where map.EurekaFieldValueDefinition.ID == eurekaValueID
                        && map.AlienFieldValueDefinition.FieldDefinition.ID == alienFieldID
                    select map.AlienFieldValueDefinition).FirstOrDefault();
        }

        /// <summary>
        /// Get all fields maps from alienFormat to eurekaFormat
        /// for specified eureka field
        /// ordered by id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaField"></param>
        /// <param name="alienFormat"></param>
        /// <returns></returns>
        public static IList<FieldValueDefinitionMap> GetValuesMaps(EurekaStrokeEntities ctx, FieldDefinition eurekaField, Format alienFormat)
        {
            return
                (from map in ctx.FieldValueDefinitionMap.Include("AlienFieldValueDefinition").Include("EurekaFieldValueDefinition")
                 where map.EurekaFieldValueDefinition.FieldDefinition.ID == eurekaField.ID
                       && map.AlienFieldValueDefinition.FieldDefinition.Format.ID == alienFormat.ID
                 orderby map.EurekaFieldValueDefinition.ID
                 select map).ToList();
        }

        /// <summary>
        /// Return a list of objects to remove for specified field (eureka or alien)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static List<EntityObject> GetFieldObjectsToRemove(FieldDefinition field)
        {
            var isEureka = field.IsNative;
            var itemsToRemove = new List<EntityObject>();

            // delete value maps and values
            field.FieldValueDefinitions.EnsureLoad();
            field.FieldValueDefinitions.ForEach(value =>
            {
                var valueMaps = value.LinkedMaps;
                valueMaps.ForEach(itemsToRemove.Add);
                itemsToRemove.Add(value);
            });

            // delete field maps
            var maps = field.LinkedMaps;
            maps.ForEach(itemsToRemove.Add);

            // delete linked alien fields
            if (isEureka)
            {
                var fields = field.LinkedFields;
                fields.ForEach(f =>
                {

                    if (f.IsAlien)
                    {
                        // recursive call to retrieve objects for removal
                        var list = GetFieldObjectsToRemove(f);
                        itemsToRemove.AddRange(list);
                    }

                }
                );
            }

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
        public static int TryDeleteFieldDefinition(EurekaStrokeEntities ctx, int id, out string msg)
        {
            msg = string.Empty;
            FieldDefinition field = null;

            try
            {
                field = GetFieldDefinitionByID(ctx, id);
                if (null == field)
                {
                    msg = "The field with ID = " + id + " does not exist";
                    return 1;
                }

                bool isEureka = field.IsNative;
                if (isEureka)
                {
                    field.StrokeFieldValues.EnsureLoad();
                    if (field.StrokeFieldValues.Count > 0)
                    {
                        msg = "The field '" + field.Name + "' cannot be deleted because it already has data";
                        return 2;
                    }
                    
                    //// how many other fields are mapped?
                    //int cnt = field.LinkedMaps.Count;
                    //if (cnt > 0)
                    //{
                    //    msg = "The field '" + field.Name + "' cannot be deleted because it has other alien fields linked";
                    //    return ok;
                    //}
                }

                // get a list of entities for removal
                var itemsToRemove = GetFieldObjectsToRemove(field);
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
        public static int TryDeleteValueDefinition(EurekaStrokeEntities ctx, int id, out string messageOut)
        {
            messageOut = string.Empty;

            var value = GetFieldValueDefinitionByID(ctx, id);
            if (null == value)
            {
                messageOut = "The value with ID = " + id + " does not exist";
                return 1;
            }

            // delete all alien values mapped to this eureka value
            string message;
            var ret = TryDeleteAlienValueDefinitions(ctx, value, out message);
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
        public static int TryDeleteAlienValueDefinitions(EurekaStrokeEntities ctx, FieldValueDefinition eurekaValue, out string msg)
        {
            msg = string.Empty;

            try
            {
                var valueMaps = eurekaValue.LinkedMaps;
                valueMaps.ForEach(map =>
                {
                    ctx.DeleteObject(map.AlienFieldValueDefinition);
                    ctx.DeleteObject(map);
                });

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




        /// <summary>
        /// Create new field definition
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FieldDefinition InsertFieldDefinition(EurekaStrokeEntities ctx, string name, short type, string description, Format format)
        {
            var field = FieldDefinition.CreateFieldDefinition(0, name, type, false, true);
            if (null == field)
                return null;
            ctx.AddToFieldDefinition(field);
            field.FormatReference.Value = format;
            field.Description = description;
            return field;
        }

        /// <summary>
        /// Map alien field definition to eureka field definition
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaField"></param>
        /// <param name="alienField"></param>
        /// <param name="regexp"></param>
        public static void MapField(EurekaStrokeEntities ctx, FieldDefinition eurekaField, FieldDefinition alienField, string regexp)
        {
            alienField.AlienFieldDefinitionMaps.Add(new FieldDefinitionMap()
                                                   {
                                                       EurekaFieldDefinition = eurekaField,
                                                       AlienFieldDefinition = alienField,
                                                       RegularExpression = regexp
                                                   });
        }

        /// <summary>
        /// Add new value to specified field
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="field"></param>
        /// <param name="strValue"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static FieldValueDefinition InsertFieldValueDefinition(EurekaStrokeEntities ctx, FieldDefinition field, string strValue, string description)
        {
            var value = new FieldValueDefinition
                            {
                                Value = strValue,
                                Description = description,
                                FieldDefinitionReference = {Value = field}
                            };
            ctx.AddToFieldValueDefinition(value);
            return value;
        }

        /// <summary>
        /// Map alien value to eureka value
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="eurekaValue"></param>
        /// <param name="alienValue"></param>
        public static void MapValue(EurekaStrokeEntities ctx, FieldValueDefinition eurekaValue, FieldValueDefinition alienValue)
        {
            alienValue.AlienFieldValueDefinitionMaps.Add(new FieldValueDefinitionMap()
                                                   {
                                                       EurekaFieldValueDefinition = eurekaValue,
                                                       AlienFieldValueDefinition = alienValue
                                                   });
        }

    }

    /// <summary>
    /// Storage for fields definitions
    /// </summary>
    public class FieldDefinitionCache
    {

        protected Dictionary<string, FieldDefinition> map = null;
        protected Format format = null;

        [ThreadStatic]
        public static FieldDefinitionCache instance = null;

        protected static object locker = new object();

        public static FieldDefinitionCache Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new FieldDefinitionCache();
                }
                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        public Dictionary<string, FieldDefinition> Load(Format format, EurekaStrokeEntities ctx)
        {
            this.format = format;
            var fields = DataFacade.GetFieldDefinitions(ctx, format,true);
            if (fields != null)
            {
                map = fields.ToDictionary<FieldDefinition, string, FieldDefinition>(f => f.Name, f => f);
            }
            return map;
        }

        public FieldDefinition this[Format format, string fieldName]
        {
            get
            {
                FieldDefinition definition = null;
                if (this.format == null || format.ID != this.format.ID)
                {
                    EurekaStrokeEntities ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
                    Load(format, ctx);
                }
                if (map != null)
                {
                    if (map.ContainsKey(fieldName))
                        definition = map[fieldName];
                }
                return definition;
            }
        }

    }


    //public class FieldDefinitionsStorage : ContextSingletonBase<Dictionary<string,FieldDefinition>>
    //{

    //    protected Format format { get; set; }
    //    protected override string key { get { return "FieldDefinitions" + format.ID.ToString(); } }

    //    protected FieldDefinitionsStorage(Format format) {

    //        this.format = format;
    //        EurekaStrokeEntities ctx = Utils.GetDataContext();
    //        var fields = DataFacade.GetFieldDefinitions(ctx, format);
    //        if (fields != null)
    //        {
    //            Subject = fields.ToDictionary<FieldDefinition,string, FieldDefinition>(f => f.Name, f=>f);
    //        }



    //    }

    //    [ThreadStatic]
    //    private static FieldDefinitionsStorage item = null;

    //    public static FieldDefinitionsStorage Current
    //    {
    //        get {

    //            return item; 

    //        }
    //        set { item = value; }
    //    }

    //    /// <summary>
    //    /// Get Field Definition
    //    /// </summary>
    //    /// <param name="format"></param>
    //    /// <param name="fieldName"></param>
    //    /// <returns></returns>
    //    public static FieldDefinition GetFieldDefinition(Format format, string fieldName)
    //    {
    //        FieldDefinition definition = null;
    //        if (item == null) 
    //            item = new FieldDefinitionsStorage(format);
    //        else if (item != null && item.format.ID != format.ID)
    //        {
    //            item = new FieldDefinitionsStorage(format);
    //        }
    //        var dict = item.Subject;
    //        if (dict != null)
    //        {
    //            if (dict.ContainsKey(fieldName))
    //                definition = dict[fieldName];
    //        }
    //        return definition;
    //    }

    //    //public FieldDefinition this[Format format, string fieldName] {
    //    //    get
    //    //    {
    //    //        FieldDefinition definition = null;
    //    //        Dictionary<string, FieldDefinition> dict = GetItem(format);
    //    //        if (dict != null)
    //    //        {
    //    //            if (dict.ContainsKey(fieldName))
    //    //                definition = dict[fieldName];
    //    //        }
    //    //        return definition;
    //    //    }
    //    //}

    //    public void Reset()
    //    {
    //        item = null;
    //    }
    //}


}
