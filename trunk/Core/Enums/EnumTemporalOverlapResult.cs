using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    /// <summary>
    /// Temporal overlap result 
    /// </summary>
    public enum EnumTemporalOverlapResult : short
    {
        Ignore = 0,
        CheckKeyFields = 1,
        NewRecord = 2
    }
}
