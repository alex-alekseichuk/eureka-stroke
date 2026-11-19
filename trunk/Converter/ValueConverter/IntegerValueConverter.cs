using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    internal class IntegerValueConverter : BaseValueConverter
    {
        
        /// <summary>
        /// Parse int value from string
        /// </summary>
        /// <param name="val"></param>
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            string value = val.RawValue;
            int res = 0;
            bool ok = int.TryParse(value, out res);
            if (!ok)
            {
                ErrorCode = -1;
                ErrorMessage = string.Format("Failure to parse value {0} as integer field {1}", value, fieldTarget.Name);
                val.Status = -1;
                ErrorType = EnumErrorType.Conversion;
            }
            else
            {
                val.IntegerValue = res;
                val.StringValue = res.ToString();
                val.Status = 0;
            }
        }
    }
}
