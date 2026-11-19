using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Eureka.Stroke.Converter
{
    /// <summary>
    /// Abstract value converter
    /// </summary>
    internal abstract class BaseValueConverter : IValueConverter
    {

        protected EurekaStrokeEntities ctx = null;

        public BaseValueConverter()
        {
        }

        /// <summary>
        /// Find a source value definition for a given raw value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldSource"></param>
        /// <returns></returns>
        protected AlienValue GetSourceValue(string value, AlienField fieldSource, EurekaField fieldTarget)
        {
            fieldTarget.AlienValue.EnsureLoad();
            AlienValue valueDefinition = (from v in fieldTarget.AlienValue
                                                    where v.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase)
                                                    select v).FirstOrDefault();

            return valueDefinition;
        }

        /// <summary>
        /// Validate raw value using regular expression
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        protected bool Validate(string rawValue, string regex)
        {
            bool ok = true;
            if (string.IsNullOrEmpty(regex)) return ok;
            else
            {
            }
            return ok;
        }

        /// <summary>
        /// Extract value using regular expression
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        protected string ExtractValue(string rawValue, string regex)
        {
            string val = rawValue;
            //if (string.IsNullOrEmpty(regex)) return val;
            //else
            //{
            //}
            return val;
        }

        /// <summary>
        /// Prepare the value before processing (validate, extract via regex)
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="field"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        protected string PrepareValue(string rawValue, AlienField field, EurekaField fieldTarget)
        {
            string value = rawValue;
            if (fieldTarget != null)
            {
                //bool ok = Validate(rawValue, field.RegularExpression);
                bool ok = true;
                if (!ok)
                {
                    return value;
                }
                value = ExtractValue(rawValue, field.RegularExpression);
            }
            else
            {
                Logger.Instance.LogInfo("No mapping data for field " + field.Name);
            }
            return value;
        }

        /// <summary>
        /// Parse raw value into proper format
        /// </summary>
        /// <param name="val"></param>
        /// <param name="fieldSource"></param>
        /// <param name="fieldTarget"></param>
        /// <param name="map"></param>
        protected abstract void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget);

        /// <summary>
        /// Get string value for further parsing to proper format
        /// </summary>
        /// <param name="val"></param>
        /// <param name="fieldSource"></param>
        /// <param name="fieldTarget"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        protected virtual string GetValueForConversion(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            string value = val.RawValue;
            if (fieldSource != null)
            {
                // TODO: change to fieldSource
                EnumFieldType typeTarget = fieldTarget.FieldTypeEnum;
                // if mapped from Static Value, use regex to retrieve value from static description (for dynamic dosage)
                //if (typeSource == EnumFieldType.SingleSelect)
                if (!string.IsNullOrEmpty(fieldSource.RegularExpression))
                {
                    string formatName = fieldSource.Format != null ? fieldSource.Format.Name : "";
                    string alienFieldDescription = "";
                    AlienValue valueSource = GetSourceValue(value, fieldSource, fieldTarget);

                    if (valueSource == null)
                    {
                        ErrorMessage = string.Format("Value {0} is not known for field '{1}', format {2}", value, fieldSource.Name, formatName);
                        ErrorCode = -1;
                        val.Status = -1;
                        ErrorType = EnumErrorType.MissingValue;
                        return value;
                    }

                    alienFieldDescription = valueSource.Description;
                    value = GetRegexValue(fieldSource.RegularExpression, alienFieldDescription);
                }
            }
            return value;
        }

        /// <summary>
        /// Get regex value
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetRegexValue(string regex, string value)
        {
            string result = value;
            try
            {
                Regex exp = new Regex(regex, RegexOptions.IgnoreCase);
                MatchCollection matchList = exp.Matches(value);
                if (matchList.Count > 0)
                {
                    Match firstMatch = matchList[0];
                    result = firstMatch.Value;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                ErrorType = EnumErrorType.Conversion;
            }
            return result;

        }

        /// <summary>
        /// Create a new StrokeFieldValue object
        /// </summary>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        protected StrokeFieldValue GetInitialValue(AlienField fieldSource, EurekaField fieldTarget, string rawValue)
        {
            var val = new StrokeFieldValue
            {
                CreatedBy = null,
                CreatedDate = DateTime.Now,
                RawValue = rawValue,
                StringValue = rawValue,

                Status = 0
            };

            //if (map != null && !string.IsNullOrEmpty(map.RegularExpression))
            //{
            //    EnumFieldType typeSource = fieldSource.FieldTypeEnum;
            //    EnumFieldType typeTarget = fieldTarget.FieldTypeEnum;
            //    if (typeSource == EnumFieldType.SingleSelect &&
            //        typeTarget == EnumFieldType.Decimal)
            //    {
            //        string alienFieldDescription = fieldSource.Description;
            //        val.RawValue = GetRegexValue(map.RegularExpression, alienFieldDescription);
            //    }
            //}

            return val;
        }

        #region IValueConverter Members

        /// <summary>
        /// Parse, validate and convert raw string value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        public virtual IEnumerable<StrokeFieldValue> Convert(AlienField fieldSource, EurekaField fieldTarget, string rawValue)
        {
            string value = PrepareValue(rawValue, fieldSource, fieldTarget);

            var val = GetInitialValue(fieldSource, fieldTarget, value);
            parseValue(val, fieldSource, fieldTarget);

            if (ctx == null)
                ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            //ctx.AddToStrokeFieldValue(val);
            val.EurekaFieldReference.Value = fieldTarget;

            StrokeFieldValue[] strokeFields = new StrokeFieldValue[]
            {
                val
            };
            return strokeFields;
        }

        public bool IsValid
        {
            get { return ErrorCode == 0; }
        }

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
        

        #endregion

        #region IError Members

        public int ErrorCode
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        #endregion
    }
}
