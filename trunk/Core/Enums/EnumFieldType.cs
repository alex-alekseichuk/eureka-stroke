using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    /// <summary>
    /// Field types
    /// </summary>
    public enum EnumFieldType : short
    {
        String = 0,
        Integer = 1,
        Boolean = 2,
        Decimal = 3,
        DateTime = 4,
        SingleSelect = 5,
        MultipleSelect = 6
    }
    public static class FieldType
    {
        public static readonly IDictionary<int, string> Types = new Dictionary<int, string>()
                                                                       {
                                                                           {0, "String"},
                                                                           {1, "Integer"},
                                                                           {2, "Boolean"},
                                                                           {3, "Decimal"},
                                                                           {4, "DateTime"},
                                                                           {5, "SingleSelect"},
                                                                           {6, "MultipleSelect"}
                                                                        };
    }
}
