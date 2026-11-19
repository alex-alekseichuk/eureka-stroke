using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;

namespace Eureka.Stroke.Converter
{

    /// <summary>
    /// Converter input data holder
    /// </summary>
    public class ConverterRequest
    {
        /// <summary>
        /// Alien field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Source (alien) field value to be converted
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// Alien format
        /// </summary>
        public Format Format { get; set; }

        public ConverterRequest(string fieldName, Format format, string value)
        {
            FieldValue = value;
            FieldName = fieldName;
            Format = format;

        }
    }

    /// <summary>
    /// Converter output data holder
    /// </summary>
    public class ConverterResponse : BaseService
    {
        /// <summary>
        /// Alien field definition
        /// </summary>
        public AlienField SourceFieldDefinition { get; set; }

        /// <summary>
        /// Target (eureka) field definition
        /// </summary>
        public EurekaField TargetFieldDefinition { get; set; }

        
        /// <summary>
        /// Converted value
        /// </summary>
        public IEnumerable<StrokeFieldValue> StrokeValues { get; set; }

        public bool IsValid
        {
            get { return ErrorCode == 0; }
        }

        public ConverterResponse(int errorCode, string errorMessage, EnumErrorType errorType)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        /// <summary>
        /// Error type
        /// </summary>
        public EnumErrorType ErrorType { get; set; }

        /// <summary>
        /// Get processing type for a given error type
        /// </summary>
        public EnumErrorProcessingType ErrorProcessingType
        {

            get
            {
                EnumErrorProcessingType type = BaseService.GetErrorProcessingType(ErrorType);
                return type;
            }
        }
    }

    /// <summary>
    /// IConverter interface
    /// </summary>
    public interface IConverter 
    {
        /// <summary>
        /// Convert one value
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        ConverterResponse ConvertValue(ConverterRequest req);
    }
}
