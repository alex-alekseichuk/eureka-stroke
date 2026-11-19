using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{

    /// <summary>
    /// Enumerates possible types of converter errors
    /// </summary>
    public enum EnumErrorType : int
    {
        
        None = 0,

        MissingField = 1,

        MissingFieldMapping = 2,

        MissingValue = 3,

        MissingValueMapping = 4,

        Conversion = 5,

        Validation = 6,

        Other = 7



    }


}
