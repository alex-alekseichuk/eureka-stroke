using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WebApplication
{

    /// <summary>
    /// CSV generator helper
    /// </summary>
    public static class CSVHelper
    {
        #region Separators
        /// <summary>
        /// CSV file column separator
        /// </summary>
        public static string COLUMN_SEPARATOR
        {
            get
            {
                return ",";
            }
        }

        /// <summary>
        /// CSV file quote string
        /// </summary>
        public static string QUOTE_STRING
        {
            get
            {
                return "\"";
            }
        }

        /// <summary>
        /// CR/LF
        /// </summary>
        public static string CRLF
        {
            get
            {
                return "\r\n";
            }
        }
        #endregion

        public static string GetLine(/*params*/ object[] Values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object o in Values)
            {
                if (sb.Length > 0) sb.Append(COLUMN_SEPARATOR);
                string value = o == null ? string.Empty : o.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                if (value.Contains(' ')
                    || value.Contains(COLUMN_SEPARATOR))
                {
                    value = string.Format("{0}{1}{0}",
                        QUOTE_STRING,
                        value.Replace(QUOTE_STRING, string.Concat(QUOTE_STRING, QUOTE_STRING)));
                }
                sb.Append(value);
            }
            return sb.ToString();
        }

        //public static string GetLine<T>(/*params*/ IEnumerable<T> Values) where T : class
        //{
        //    return GetLine(Values.Select(t => t.ToString()).ToArray());
        //}
    }
}