using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    public enum EnumErrorProcessingType
    {
        /// <summary>
        /// Error ignored
        /// </summary>
        None = 0,

        /// <summary>
        /// Error ignored
        /// </summary>
        Ignore = 1,

        /// <summary>
        /// Error captured and counted
        /// </summary>
        Capture = 2
    }
}
