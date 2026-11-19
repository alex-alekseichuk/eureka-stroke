using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Eureka.Stroke.Converter
{
    internal class DecimalValueConverter : BaseValueConverter
    {

        /// <summary>
        /// Parse int value from string
        /// </summary>
        /// <param name="val"></param>
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            // get value applicable for further conversion
            string value = GetValueForConversion(val, fieldSource, fieldTarget);
            decimal res = 0;

            CultureInfo ci = CultureInfo.InvariantCulture;
            NumberStyles styles = NumberStyles.Number;
            bool ok = decimal.TryParse(value, styles,ci,out res);
            if (!ok)
            {
                ErrorCode = -1;
                ErrorMessage = string.Format("Decimal: Failure to parse value {0} as decimal field {1}", value, fieldTarget.Name);
                val.Status = -1;
                ErrorType = EnumErrorType.Conversion;
            }
            else
            {
                val.DecimalValue = res;
                val.Status = 0;
                val.StringValue = res.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}