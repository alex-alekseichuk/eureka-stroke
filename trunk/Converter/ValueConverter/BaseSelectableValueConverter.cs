using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    /// <summary>
    /// Base class for handling selectable types (SingleSelect and MultipleSelect)
    /// </summary>
    internal abstract class BaseSelectableValueConverter : BaseValueConverter
    {

        /// <summary>
        /// Parse single select value from string
        /// </summary>
        /// <param name="val"></param>
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            string msg = string.Empty;
            string formatName = string.Empty;
            string value = val.RawValue;
            int res = 0;
            formatName = fieldSource.Format != null ? fieldSource.Format.Name : "";
            AlienValue valueSource = GetSourceValue(value, fieldSource, fieldTarget);

            if (valueSource == null)
            {
                ErrorMessage = string.Format("Value {0} is not known for field '{1}', format {2}", value, fieldSource.Name, formatName);
                ErrorCode = -1;
                val.Status = -1;
                ErrorType = EnumErrorType.MissingValue;
                return;
            }

            EurekaValue valueTarget = GetTargetFieldValue(valueSource);
            if (valueTarget == null)
            {
                ErrorMessage = string.Format("Mapping from format {0} to format {1} for Value {2} (Field '{3}') is not found ", formatName, "Eureka", value, fieldTarget.Name);
                ErrorCode = -1;
                val.Status = -1;
                ErrorType = EnumErrorType.MissingValueMapping;
                return;
            }

            // get mapped value
            string mappedValue = valueTarget.Value;

            bool ok = int.TryParse(mappedValue, out res);
            if (!ok)
            {
                ErrorCode = 0;
                ErrorMessage = string.Format("SingleSelect Warning: Failure to parse value {0} as integer field {1}", mappedValue, fieldTarget.Name);
                val.Status = 0;
            }
            else
            {
                val.IntegerValue = res;
                val.Status = 0;
            }
            val.StringValue = mappedValue;
        }

        
        /// <summary>
        /// Find a target field value definition
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        protected virtual EurekaValue GetTargetFieldValue(AlienValue sourceValue)
        {
            EurekaValue targetValue = null;
            //map = GetFieldValueMap(sourceValue);
            if (sourceValue != null)
            {
                sourceValue.EurekaValueReference.EnsureLoad();
                targetValue = sourceValue.EurekaValue;
            }
            return targetValue;
        }


    }
}
