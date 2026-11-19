using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;
using Microsoft.Practices.Unity;

namespace Eureka.Stroke.FormatService
{
    /// <summary>
    /// Implementation of Format Manager Service.
    /// It's used to handle meta data that the parser-converter uses.
    /// </summary>
    public class FormatService : BaseService, IFormatService
    {
        #region Eureka Format

        /// <summary>
        /// The name of the Eureka Format in the database
        /// The Name of Eureka format is set in configuration file
        /// </summary>
        public readonly string EurekaFormatName;

        /// <summary>
        /// Eureka Format Entity
        /// It's configured and loaded one time
        /// </summary>
        private Format _EurekaFormat;
        public Format EurekaFormat
        {
            get
            {
                if (null == _EurekaFormat)
                {
                    // load Eureka Format by first time access
                    var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
                    _EurekaFormat = DataFacade.GetFormatByName(ctx, EurekaFormatName);
                }
                return _EurekaFormat;
            }
        }

        /// <summary>
        /// Constructor that need to know the name of Eureka Format
        /// </summary>
        /// <param name="eurekaFormatName"></param>
        public FormatService([Dependency("EurekaFormatName")] string eurekaFormatName)
        {
            EurekaFormatName = eurekaFormatName;
        }

        #endregion

        #region formats

        /// <summary>
        /// Getting list of all Alien formats with status New or Existing
        /// </summary>
        /// <returns></returns>
        public IList<Format> GetFormats()
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            return DataFacade.GetFormats(ctx);
        }

        /// <summary>
        /// Get specified Format by its ID
        /// </summary>
        /// <param name="formatID"></param>
        /// <returns>Detached Format object</returns>
        public Format GetFormatByID(int formatID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var format = DataFacade.GetFormatByID(ctx, formatID);
            var keys = new List<EurekaField>();
            if (format != null)
            {

                try
                {
                    // Detach each item from the collection.
                    while (format.KeyEurekaField.Count > 0)
                    {
                        // Detach the first SalesOrderDetail in the collection.
                        var key = format.KeyEurekaField.First();
                        keys.Add(key);
                        ctx.Detach(key);
                    }

                    // Detach the format
                    ctx.Detach(format);

                    keys.ForEach(format.KeyEurekaField.Add);
                }
                catch (InvalidOperationException ex)
                {
                    //Console.WriteLine(ex.ToString());
                }

                //format.KeyEurekaField.ForEach(ctx.Detach);
                //ctx.Detach(format);
            }
            return format;
        }

        /// <summary>
        /// Get New or Existing Format by its Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Detached Format object</returns>
        public Format GetFormatByName(string name)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            var format = DataFacade.GetFormatByName(ctx, name);
            ctx.Detach(format);
            return format;
        }

        /// <summary>
        /// Add new Format
        /// </summary>
        /// <param name="name">Name of format</param>
        /// <returns>New Format object or null</returns>
        public Format AddFormat(string name)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                ErrorCode = 2;
                ErrorMessage = "Format Name is not provided";
                return null;
            }
            if (name.Length > 100)
            {
                ErrorCode = 3;
                ErrorMessage = "Format Name may have length max 100 chars";
                return null;
            }

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            if (null != DataFacade.GetFormatByName(ctx, name))
            {
                ErrorCode = 1;
                ErrorMessage = "Format with such Name already exists";
                return null;
            }

            return DataFacade.AddFormat(ctx, name);
        }

        /// <summary>
        /// Internal implementation of changing Format status
        /// 0-New, 1-Existing, 2-Deleted
        /// </summary>
        /// <param name="formatID"></param>
        /// <param name="status"></param>
        protected void SetFormatStatus(int formatID, short status)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (status < 0 || status > 2)
            {
                ErrorCode = 2;
                ErrorMessage = "Incorrect Format Status value: " + status;
                return;
            }

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var format = DataFacade.GetFormatByID(ctx, formatID);
            if (null == format)
            {
                ErrorCode = 1;
                ErrorMessage = "No such Format";
                return;
            }

            // avoid disabling not-new format
            if (2 == status && 1 == format.Status)
            {
                ErrorCode = 3;
                ErrorMessage = "This Format can't be deleted";
                return;
            }
            DataFacade.SetFormatStatus(ctx, format, status);
        }

        /// <summary>
        /// Turn Format into Disabled (Deleted) status.
        /// It may be done for New Format only.
        /// </summary>
        /// <param name="formatID"></param>
        public void DisableFormat(int formatID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            SetFormatStatus(formatID, 2);
        }

        /// <summary>
        /// Turn Format into Existing status.
        /// It may be done right after some stroke data added to DB by this Format.
        /// </summary>
        /// <param name="formatID"></param>
        public void MakeFormatExisting(int formatID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            SetFormatStatus(formatID, 1);
        }

        #endregion

        #region eureka fields

        public bool HasAlienFields(int eurekaFieldID, int formatID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // get all native fields
            return DataFacade.GetAlienFieldDefinitionsForEurekaField(
                    ctx, 
                    DataFacade.GetEurekaFieldDefinitionByID(ctx, eurekaFieldID), 
                    DataFacade.GetFormatByID(ctx, formatID)
                ).Count > 0;
        }

        /// <summary>
        /// Get all Eureka fields of specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public List<EurekaField> GetEurekaFields(Format format)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == format)
                throw new ArgumentOutOfRangeException("format");

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // get all native fields
            return DataFacade.GetEurekaFieldDefinitions(ctx, EurekaFormat).ToList();
        }

        /// <summary>
        /// Get detached entity by ID.
        /// It's detached for later use in upper layer.
        /// </summary>
        /// <param name="eurekaFieldID"></param>
        /// <returns>Detached EurekaField object</returns>
        public EurekaField GetEurekaFieldByID(int eurekaFieldID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var eurekaFieldDef = DataFacade.GetEurekaFieldDefinitionByID(ctx, eurekaFieldID);
            if (null == eurekaFieldDef)
            {
                ErrorCode = 1;
                ErrorMessage = "No such Field";
                return null;
            }
            ctx.Detach(eurekaFieldDef);
            return eurekaFieldDef;
        }

        /// <summary>
        /// Delete EurekaField and all its child objects (EurekaValue, AlienField, AlienValue)
        /// </summary>
        /// <param name="eurekaField"></param>
        public void DeleteEurekaField(EurekaField eurekaField)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;
            if (null == eurekaField)
                throw new ArgumentOutOfRangeException("eurekaField");
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            string msg;
            ErrorCode = DataFacade.TryDeleteEurekaFieldDefinition(ctx, eurekaField.ID, out msg);
            ErrorMessage = msg;
        }

        /// <summary>
        /// Check incoming Eureka Field data
        /// Set ErrorCode and ErrorMessage if there is some validation issue.
        /// </summary>
        /// <param name="eurekaField"></param>
        /// <returns>true if Eureka Field data is valid</returns>
        private bool CheckEurekaField(string Name, string Description)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorCode = 1;
                ErrorMessage = "Eureka Name is not provided";
                return false;
            }
            if (Name.Length > 64)
            {
                ErrorCode = 2;
                ErrorMessage = "Eureka Name may have length max 64 chars";
                return false;
            }
            if (!string.IsNullOrEmpty(Description) && Description.Length > 100000)
            {
                ErrorCode = 3;
                ErrorMessage = "Eureka Description may have length max 100000 chars";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Insert new EurekaField record and fill it's ID attribute
        /// </summary>
        /// <param name="eurekaField"></param>
        /// <returns>Index of new inserted record in sorted by Name view</returns>
        public int InsertEurekaField(EurekaField eurekaField)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == eurekaField)
                throw new ArgumentOutOfRangeException("eurekaField");

            if (!CheckEurekaField(eurekaField.Name, eurekaField.Description))
                return 0;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // check if Name of field is already used
            if (null != DataFacade.GetEurekaFieldDefinition(ctx, eurekaField.Name, EurekaFormat))
            {
                ErrorCode = 4;
                ErrorMessage = "Eureka Field with such Name already exists";
                return 0;
            }

            eurekaField.Format = EurekaFormat;
            eurekaField.IsNew = true;

            DataFacade.SaveEurekaFieldDefinition(ctx, eurekaField);
            if (0 == eurekaField.ID)
            {
                ErrorCode = 5;
                ErrorMessage = "Can't save Eureks field";
                return 0;
            }

            // calcualte it's index in the sorted by Name list
            return ctx.GetEurekaFieldIndex(eurekaField.ID);
        }

        /// <summary>
        /// Update existing EurekaField data
        /// </summary>
        public void UpdateEurekaField(int eurekaFieldID, short FieldType, string Name, string Description, bool isKeyField, int keyFormatID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (!CheckEurekaField(Name, Description))
                return;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var eurekaField = DataFacade.GetEurekaFieldDefinitionByID(ctx, eurekaFieldID);
            if (null == eurekaField)
            {
                ErrorCode = 4;
                ErrorMessage = "No such Eureka Field";
                return;
            }

            // check if Name of field is already used
            var existingField = DataFacade.GetEurekaFieldDefinition(ctx, Name, EurekaFormat);
            if (null != existingField && existingField.ID != eurekaField.ID)
            {
                ErrorCode = 2;
                ErrorMessage = "Eureka Field with such Name already exists";
                return;
            }

            eurekaField.FieldType = FieldType;
            eurekaField.Name = Name;
            eurekaField.Description = Description;

            // add/remove this EurekaField as key field for specified format
            var format = DataFacade.GetFormatByID(ctx, keyFormatID);
            if (isKeyField)
            {
                if (!eurekaField.KeyFormat.Contains(format))
                    eurekaField.KeyFormat.Add(format);
            } else
            {
                if (eurekaField.KeyFormat.Contains(format))
                    eurekaField.KeyFormat.Remove(format);
            }

            // save updated eureka field
            DataFacade.SaveEurekaFieldDefinition(ctx, eurekaField);
            if (0 == eurekaField.ID)
            {
                ErrorCode = 1;
                ErrorMessage = "Can't save Eureka field";
            }
        }

        #endregion
        
        #region alien fields

        /// <summary>
        /// Get list of all Alien Fields linked to specified Eureka Field under specified Alien Format.
        /// </summary>
        /// <param name="eurekaField">Parent Eureka Field</param>
        /// <param name="format">Alien Format</param>
        /// <returns></returns>
        public IList<AlienField> GetAlienFields(EurekaField eurekaField, Format format)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // get all alien fields for this eureka field
            return DataFacade.GetAlienFieldDefinitionsForEurekaField(ctx, eurekaField, format).ToList();
        }

        /// <summary>
        /// Get Alien Field by its ID
        /// </summary>
        /// <param name="alienFieldID"></param>
        /// <returns>Detached Alien Field entity</returns>
        public AlienField GetAlienFieldByID(int alienFieldID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var alienField = DataFacade.GetAlienFieldDefinitionByID(ctx, alienFieldID);
            if (null == alienField)
            {
                ErrorCode = 1;
                ErrorMessage = "No such Field";
                return null;
            }
            ctx.Detach(alienField);
            return alienField;
        }

        /// <summary>
        /// Delete Alien Field and all its child entities
        /// </summary>
        /// <param name="alienField"></param>
        public void DeleteAlienField(AlienField alienField)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == alienField)
                throw new ArgumentOutOfRangeException("alienField");
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            string msg;
            ErrorCode = DataFacade.TryDeleteAlienFieldDefinition(ctx, alienField.ID, out msg);
            ErrorMessage = msg;
        }

        /// <summary>
        /// Check incoming Alien Field data
        /// Set ErrorCode and ErrorMessage if there is some validation issue.
        /// </summary>
        /// <param name="eurekaField"></param>
        /// <returns>true if AlienField data is valid</returns>
        private bool CheckAlienField(string Name, string RegularExpression)
        {
            if (string.IsNullOrEmpty(Name))
            {
                ErrorCode = 1;
                ErrorMessage = "Alien Field Name is not provided";
                return false;
            }
            if (Name.Length > 64)
            {
                ErrorCode = 2;
                ErrorMessage = "Alien Field Name may have length max 64 chars";
                return false;
            }

            if (!string.IsNullOrEmpty(RegularExpression) && RegularExpression.Length > 64)
            {
                ErrorCode = 3;
                ErrorMessage = "Regular Expression may have length max 64 chars";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add new Alien Field
        /// </summary>
        /// <param name="eurekaFieldID">Parent Eureka Field ID</param>
        /// <param name="formatID">Alien Format ID</param>
        /// <param name="alienField">Data of Alien Field</param>
        public void InsertAlienField(int eurekaFieldID, int formatID, AlienField alienField)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (0 == eurekaFieldID)
                throw new ArgumentOutOfRangeException("eurekaFieldID");
            if (0 == formatID)
                throw new ArgumentOutOfRangeException("formatID");
            if (null == alienField)
                throw new ArgumentOutOfRangeException("alienField");

            if (!CheckAlienField(alienField.Name, alienField.RegularExpression))
                return;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var alienFormat = DataFacade.GetFormatByID(ctx, formatID);
            if (null == alienFormat)
            {
                ErrorCode = 5;
                ErrorMessage = "No Format with such ID";
                return;
            }

            var eurekaField = DataFacade.GetEurekaFieldDefinitionByID(ctx, eurekaFieldID);
            if (null == eurekaField)
            {
                ErrorCode = 4;
                ErrorMessage = "No Eureka Field with such ID";
                return;
            }

            if (DataFacade.GetAlienFieldDefinitionsForEurekaField(ctx, eurekaField, alienFormat).Count > 0)
            {
                ErrorCode = 7;
                ErrorMessage = "Only single Alien Field may be mapped to specified Eureka Field";
                return;
            }

            // check if Name of field is already used
            var alienFieldExisting = DataFacade.GetAlienFieldDefinition(ctx, alienField.Name, alienFormat);
            if (null != alienFieldExisting)
            {
                ErrorCode = 6;
                ErrorMessage = "Alien Field with such Name already mapped to Eureka Field " + alienFieldExisting.EurekaField.Name;
                return;
            }

            alienField.EurekaField = eurekaField;
            alienField.Format = alienFormat;
            DataFacade.SaveAlienFieldDefinition(ctx, alienField);

            if (0 == alienField.ID)
            {
                ErrorCode = 8;
                ErrorMessage = "Can't save Alien field";
            }
        }

        /// <summary>
        /// Update existing Alien Field with new data
        /// </summary>
        public void UpdateAlienField(int alienFieldID, string Name, string RegularExpression)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (0 == alienFieldID)
                throw new ArgumentOutOfRangeException("alienField");

            if (!CheckAlienField(Name, RegularExpression))
                return;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            var alienField = DataFacade.GetAlienFieldDefinitionByID(ctx, alienFieldID);
            if (null == alienField)
            {
                ErrorCode = 4;
                ErrorMessage = "No such Alien Field";
                return;
            }   

            // check if Name of field is already used
            var existingField = DataFacade.GetAlienFieldDefinition(ctx, Name, alienField.Format);
            if (null != existingField && existingField.ID != alienField.ID)
            {
                ErrorCode = 5;
                ErrorMessage = "Alien Field with such Name already mapped to Eureka Field " + existingField.EurekaField.Name;
                return;
            }

            alienField.Name = Name;
            alienField.RegularExpression = RegularExpression;

            if (! DataFacade.SaveAlienFieldDefinition(ctx, alienField)
                ||
                0 == alienField.ID)
            {
                ErrorCode = 6;
                ErrorMessage = "Can't save Alien Field";
            }
        }

        #endregion
        
        #region values mapping

        /// <summary>
        /// Get the list of ValuesMapping pairs for specified Eureka Field.
        /// One of pair side is optional.
        /// But not both.
        /// It means that it's ok to have EurekaValue without mapped AlienValue
        /// or AlienValue without EurekaValue.
        /// </summary>
        /// <param name="eurekaField"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public IList<ValueMap> GetValuesMap(EurekaField eurekaField, Format format)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == eurekaField)
                throw new ArgumentOutOfRangeException("eurekaField");
            if (null == format)
                throw new ArgumentOutOfRangeException("format");

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            // get all native values
            var eurekaValues = DataFacade.GetEurekaValues(ctx, eurekaField);

            // get all alien values
            var eurekaAlienValues = DataFacade.GetEurekaAlienValues(ctx, eurekaField, format);

            // map alien values into native eureka values
            var valuesMaps = new List<ValueMap>();
            var enumeratorEureka = eurekaValues.GetEnumerator();
            var enumeratorAlien = eurekaAlienValues.GetEnumerator();

            var hasAlien = enumeratorAlien.MoveNext();
            while (enumeratorEureka.MoveNext())
            {
                var eurekaValue = enumeratorEureka.Current;

                var valueMap = new ValueMap()
                {
                    AlienID = 0,
                    AlienValue = null,
                    EurekaID = eurekaValue.ID,
                    EurekaValue = eurekaValue.Value,
                    EurekaDescription = eurekaValue.Description,
                    AlienDescription = null
                };
                while (
                    hasAlien
                    && null != enumeratorAlien.Current.EurekaValue
                    && enumeratorAlien.Current.EurekaValue.ID == eurekaValue.ID)
                {
                    valueMap.AlienID = enumeratorAlien.Current.ID;
                    valueMap.AlienValue = enumeratorAlien.Current.Value;
                    valueMap.AlienDescription = enumeratorAlien.Current.Description;
                    hasAlien = enumeratorAlien.MoveNext();
                }
                valuesMaps.Add(valueMap);
            }

            // add alien values that are not linked to any eureka value
            var unmappedAlienValues = DataFacade.GetUnmappedAlienValues(ctx, eurekaField, format);
            enumeratorAlien = unmappedAlienValues.GetEnumerator();
            while (enumeratorAlien.MoveNext())
            {
                valuesMaps.Add(new ValueMap()
                {
                    AlienID = enumeratorAlien.Current.ID,
                    AlienValue = enumeratorAlien.Current.Value,
                    EurekaID = 0,
                    EurekaValue = null,
                    EurekaDescription = null,
                    AlienDescription = null
                });
            }

            return valuesMaps;
        }

        /// <summary>
        /// Load ValuesMappign pair by ID's pair
        /// </summary>
        /// <param name="EurekaID"></param>
        /// <param name="AlienID"></param>
        /// <returns></returns>
        public ValueMap GetValueMap(int EurekaID, int AlienID)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();

            if (0 == EurekaID && 0 == AlienID)
                throw new ArgumentOutOfRangeException("EurekaID");

            var map = new ValueMap()
            {
                AlienID = 0,
                AlienValue = null,
                EurekaID = 0,
                EurekaValue = null,
                EurekaDescription = null,
                AlienDescription = null
            };
            if (0 != EurekaID)
            {
                var eurekaValue = DataFacade.GetEurekaFieldValueByID(ctx, EurekaID);
                if (null != eurekaValue)
                {
                    map.EurekaID = eurekaValue.ID;
                    map.EurekaValue = eurekaValue.Value;
                    map.EurekaDescription = eurekaValue.Description;
                }
            }
            if (0 != AlienID)
            {
                var alienValue = DataFacade.GetAlienFieldValueByID(ctx, AlienID);
                if (null != alienValue)
                {
                    map.AlienID = alienValue.ID;
                    map.AlienValue = alienValue.Value;
                    map.AlienDescription = alienValue.Description;
                }
            }
            return map;
        }

        /// <summary>
        /// Full Delete ValuesMap.
        /// Will be deleted Eureka Value and all Alien Values of all Formats linked to this Eureka Value.
        /// </summary>
        /// <param name="valueMap"></param>
        public void DeleteValueMap(ValueMap valueMap)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == valueMap)
                throw new ArgumentOutOfRangeException("valueMap");

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();


            string msg;
            if (0 != valueMap.EurekaID)
            {
                ErrorCode = DataFacade.TryDeleteEurekaValue(ctx, valueMap.EurekaID, out msg);
                ErrorMessage = msg;
                return;
            } else
                if (0 != valueMap.AlienID)
                {
                    ErrorCode = DataFacade.TryDeleteAlienValue(ctx, valueMap.AlienID, out msg);
                    ErrorMessage = msg;
                    return;
                }

            ErrorCode = 10;
            ErrorMessage = "Unknown value";
        }


        /// <summary>
        /// Save value mapping. It means pair of Eureka Value and Alien Value.
        /// Both are optional but not simultaneously.
        /// So, we save one of them or both.
        /// </summary>
        /// <param name="eurekaField"></param>
        /// <param name="format"></param>
        /// <param name="valueMap"></param>
        public void SaveValueMap(int eurekaFieldID, int formatID, ValueMap valueMap)
        {
            ErrorCode = 0;
            ErrorMessage = string.Empty;

            if (null == valueMap)
                throw new ArgumentOutOfRangeException("valueMap");
            if (0 == eurekaFieldID)
                throw new ArgumentOutOfRangeException("eurekaFieldID");
            if (0 == formatID)
                throw new ArgumentOutOfRangeException("formatID");

            if (string.IsNullOrEmpty(valueMap.EurekaValue) && string.IsNullOrEmpty(valueMap.AlienValue))
            {
                ErrorCode = 1;
                ErrorMessage = "Empty mapping is not allowed";
                return;
            }
            if (!string.IsNullOrEmpty(valueMap.EurekaValue) && valueMap.EurekaValue.Length > 64)
            {
                ErrorCode = 2;
                ErrorMessage = "Eureka Value may have length max 64 chars";
                return;
            }
            if (!string.IsNullOrEmpty(valueMap.AlienValue) && valueMap.AlienValue.Length > 64)
            {
                ErrorCode = 3;
                ErrorMessage = "Alien Value may have length max 64 chars";
                return;
            }
            if (!string.IsNullOrEmpty(valueMap.EurekaDescription) && valueMap.EurekaDescription.Length > 100000)
            {
                ErrorCode = 4;
                ErrorMessage = "Eureka Description may have length max 100000 chars";
                return;
            }
            if (!string.IsNullOrEmpty(valueMap.AlienDescription) && valueMap.AlienDescription.Length > 100000)
            {
                ErrorCode = 5;
                ErrorMessage = "Alien Definition may have length max 100000 chars";
                return;
            }

            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            string msg;

            var eurekaField = DataFacade.GetEurekaFieldDefinitionByID(ctx, eurekaFieldID);
            if (null == eurekaField)
            {
                ErrorCode = 6;
                ErrorMessage = "No such Eureka Field ID";
                return;
            }
            var alienFormat = DataFacade.GetFormatByID(ctx, formatID);
            if (null == alienFormat)
            {
                ErrorCode = 7;
                ErrorMessage = "No such Format ID";
                return;
            }

            EurekaValue eurekaValue = null;

            // no existing eureka value
            if (0 == valueMap.EurekaID)
            {
                // Eureka Value is not provided
                if (string.IsNullOrEmpty(valueMap.EurekaValue))
                {
                    // no eureka value in the map
                    // do nothing about it for now
                }
                else // there is some Eureka Value provided
                {
                    // check if such eureka value value alredy used for this eureka field
                    var existingEurekaValue = DataFacade.GetEurekaFieldValueByValue(ctx, valueMap.EurekaValue, eurekaField.ID);
                    if (null != existingEurekaValue)
                    {
                        ErrorCode = 10;
                        ErrorMessage = "Such Eureka Value already exists";
                        return;
                    }

                    // new eureka value
                    eurekaValue = EurekaValue.CreateEurekaValue(0, valueMap.EurekaValue);
                    eurekaValue.Description = valueMap.EurekaDescription;
                    eurekaValue.EurekaField = eurekaField;
                    DataFacade.SaveEurekaValueDefinition(ctx, eurekaValue);
                }
            } else // there is existing eureka value
            {
                // Eureka Value is not provided
                if (string.IsNullOrEmpty(valueMap.EurekaValue))
                {
                    // don't touch eureka value
                    // means we split values mapping row in 2 rows
                }
                else // there is some Eureka Value provided
                {
                    eurekaValue = DataFacade.GetEurekaFieldValueByID(ctx, valueMap.EurekaID);
                    if (null == eurekaValue)
                    {
                        ErrorCode = 8;
                        ErrorMessage = "No such eureka value";
                        return;
                    }

                    // check if such eureka value value alredy used for this eureka field
                    var existingEurekaValue = DataFacade.GetEurekaFieldValueByValue(ctx, valueMap.EurekaValue, eurekaField.ID);
                    if (null != existingEurekaValue && existingEurekaValue.ID != eurekaValue.ID)
                    {
                        ErrorCode = 10;
                        ErrorMessage = "Such Eureka Value already exists";
                        return;
                    }

                    eurekaValue.Value = valueMap.EurekaValue;
                    eurekaValue.Description = valueMap.EurekaDescription;
                    DataFacade.SaveEurekaValueDefinition(ctx, eurekaValue);
                }
            }


            AlienValue alienValue = null;
            if (0 == valueMap.AlienID) // no existing alien value
            {
                if (string.IsNullOrEmpty(valueMap.AlienValue))
                {
                    // no alien value in the map
                }
                else
                {
                    var existingAlienValue = DataFacade.GetAlienFieldValueByValue(ctx, valueMap.AlienValue, eurekaField.ID, alienFormat.ID);
                    if (null != existingAlienValue)
                    {
                        ErrorCode = 11;
                        ErrorMessage = "Such Alien Value already exists";
                        return;
                    }

                    // new alien value
                    alienValue = AlienValue.CreateAlienValue(0, valueMap.AlienValue, valueMap.AlienDescription ?? "");
                    alienValue.Format = alienFormat;
                    alienValue.EurekaField = eurekaField;

                    // map to eurekaValue
                    if (null != eurekaValue)
                        alienValue.EurekaValue = eurekaValue;

                    DataFacade.SaveAlienValueDefinition(ctx, alienValue);
                }
            }
            else // there is existing alien value
            {
                if (string.IsNullOrEmpty(valueMap.AlienValue))
                {
                    // just remove alien value
                    DataFacade.TryDeleteAlienValue(ctx, valueMap.AlienID, out msg);
                }
                else
                {
                    alienValue = DataFacade.GetAlienFieldValueByID(ctx, valueMap.AlienID);
                    if (null == alienValue)
                    {
                        ErrorCode = 9;
                        ErrorMessage = "No such alien value";
                        return;
                    }

                    var existingAlienValue = DataFacade.GetAlienFieldValueByValue(ctx, valueMap.AlienValue, eurekaField.ID, alienFormat.ID);
                    if (null != existingAlienValue && existingAlienValue.ID != alienValue.ID)
                    {
                        ErrorCode = 11;
                        ErrorMessage = "Such Alien Value already exists";
                        return;
                    }

                    alienValue.Value = valueMap.AlienValue;
                    alienValue.Description = valueMap.AlienDescription ?? "";

                    // map to eurekaValue
                    // or unmap it (=null)
                    alienValue.EurekaValue = eurekaValue;

                    DataFacade.SaveAlienValueDefinition(ctx, alienValue);
                }
            }

        }

        #endregion
    }
}

