using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    internal class BooleanValueConverter : BaseValueConverter
    {

        /// <summary>
        /// Parse bool value from string
        /// </summary>
        /// <param name="val"></param>
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            string value = val.RawValue.ToLowerInvariant();
            bool res = false;
            bool ok = true;

            if (value == "yes" || value == "y" || value == "true" || value == "1")
                res = true;
            else if (string.IsNullOrEmpty(value) || value == "no" || value == "n" || value == "false" || value == "0")
                res = false;
            else
            {
                ok = false;

            }
                        
            if (!ok)
            {
                ErrorCode = -1;
                ErrorMessage = string.Format("Failure to parse value {0} as boolean", value);
                val.Status = -1;
                ErrorType = EnumErrorType.Conversion;
            }
            else
            {
                val.BooleanValue = res;
                val.StringValue = res.ToString();
                val.Status = 0;
            }
        }
    }
}
