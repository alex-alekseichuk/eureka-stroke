using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.FormatService
{
    /// <summary>
    /// Interface of the Format Manager.
    /// It's used to handle metadata for Parser-Converter.
    /// </summary>
    public interface IFormatService : IError
    {
        #region Format

        /// <summary>
        /// Our Eureka Native Format
        /// </summary>
        Format EurekaFormat { get; }

        /// <summary>
        /// Get list of all formats
        /// </summary>
        /// <returns>List of all formats</returns>
        IList<Format> GetFormats();

        /// <summary>
        /// Get Format by ID
        /// </summary>
        /// <param name="formatID"></param>
        /// <returns></returns>
        Format GetFormatByID(int formatID);

        /// <summary>
        /// Get Format by its Name
        /// </summary>
        /// <param name="name">Format Name</param>
        /// <returns></returns>
        Format GetFormatByName(string name);

        /// <summary>
        /// Add new Format
        /// </summary>
        /// <param name="name">Name of format</param>
        Format AddFormat(string name);

        /// <summary>
        /// Disable specified format.
        /// So, it can't be used in FMS and Uploader.
        /// It's like Remove Format but not from DB.
        /// </summary>
        /// <param name="formatID"></param>
        void DisableFormat(int formatID);

        /// <summary>
        /// Set format status as Existing.
        /// That means that there are some parsed data under this format.
        /// By default every new format is New.
        /// It's ok to remove (disable) a New format in FMS
        /// but not Existing one.
        /// </summary>
        /// <param name="formatID"></param>
        void MakeFormatExisting(int formatID);

        #endregion

        bool HasAlienFields(int eurekaFieldID, int formatID);

        /// <summary>
        /// Get all Eureka fields
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        List<EurekaField> GetEurekaFields(Format format);
        EurekaField GetEurekaFieldByID(int eurekaFieldID);
        void DeleteEurekaField(EurekaField field);
        int InsertEurekaField(EurekaField field);
        void UpdateEurekaField(int eurekaFieldID, short FieldType, string Name, string Description, bool isKeyField, int keyFormatID);

        IList<AlienField> GetAlienFields(EurekaField eurekaField, Format format);
        AlienField GetAlienFieldByID(int alienFieldID);
        void DeleteAlienField(AlienField alienField);
        void InsertAlienField(int eurekaFieldID, int formatID, AlienField alienField);
        void UpdateAlienField(int alienFieldID, string Name, string RegularExpression);

        IList<ValueMap> GetValuesMap(EurekaField eurekaField, Format format);
        ValueMap GetValueMap(int eurekaValueID, int alienValueID);
        void DeleteValueMap(ValueMap valueMap);
        void SaveValueMap(int eurekaFieldID, int formatID, ValueMap valueMap);

    }
}
