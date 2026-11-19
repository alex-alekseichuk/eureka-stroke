using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    internal class StringValueConverter : BaseValueConverter
    {
        protected override void parseValue(StrokeFieldValue val, AlienField fieldSource, EurekaField fieldTarget)
        {
            // do nothing for string type
        }

        
    }
}
