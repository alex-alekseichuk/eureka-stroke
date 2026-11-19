using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Eureka.Stroke.Converter
{
    internal class DateTimeValueConverter : BaseValueConverter
    {

        /// <summary>
        /// supported date formats
        /// </summary>
        private readonly string[] formats = new string[] {
            "MM/dd/yyyy HH:mm",
            "MM/dd/yyyy",
            "M/d/yyyy",
            "yyyy/MM/dd HH:mm",
            "yyyy/dd/MM HH:mm",
            "yyyyMMddHHmmss",
            "MM/dd/yyyy hh:mm",
            "M/d/yyyy HH:mm",
            "M/d/yyyy h:mm",
            "MM/d/yyyy",
            "M/dd/yyyy",

        };

        /// <summary>
        /// Parse int value from string
        /// </summary>
        /// <param name="val"></param>
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            bool ok = true;
            string value = val.RawValue;
            DateTime res = Utils.GetDateMinValue();
            CultureInfo ci = CultureInfo.InvariantCulture;
            ok = TryParseDate(value, out res);
            if (!ok)
            {
                ErrorCode = -1;
                ErrorMessage = string.Format("Failure to parse value {0} as datetime", value);
                val.Status = -1;
                ErrorType = EnumErrorType.Conversion;
            }
            else
            {
                val.DateTimeValue = res;
                val.StringValue = res.ToString("MM/dd/yyyy HH:mm");
                val.Status = 0;
            }
        }

        /// <summary>
        /// Parse datetime from string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool TryParseDate(string value, string format, out DateTime dt)
        {
            bool ok = true;
            try
            {
                DateTimeStyles styles = DateTimeStyles.AssumeLocal;
                CultureInfo ci = CultureInfo.InvariantCulture;
                dt = DateTime.ParseExact(value, format, ci, styles);
            }
            catch (Exception ex)
            {
                ok = false;
                dt = Utils.GetDateMinValue();
            }
            return ok;

        }

        /// <summary>
        /// Try parsing date from string using all supported formats
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool TryParseDate(string value, out DateTime dt)
        {
            bool ok = false;
            dt = Utils.GetDateMinValue();
            foreach(string fmt in formats)
            {
                ok = TryParseDate(value, fmt, out dt);
                if (ok) break;
            }
            return ok;
        }


    }
}