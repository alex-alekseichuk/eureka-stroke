using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    /// <summary>
    /// Items audit types
    /// </summary>
    public enum EnumItemType : short
    {
        StrokeValue = 0,
        FieldName = 1,
        FieldType = 2,
        FieldMandatory = 3,
        FieldRegularExpression = 4,
        FieldMapping = 5,
        FieldDescription = 6,

        FieldValue = 10,
        FieldValueDescription = 11,
        FieldValueMapping = 12,
        Format = 13,
        Facility = 14
    }
}
