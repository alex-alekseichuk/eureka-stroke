using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    internal class MultipleSelectValueConverter : BaseSelectableValueConverter
    {

        /// <summary>
        /// Parse, validate and convert raw string value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="rawValue"></param>
        /// <returns></returns>
        public override IEnumerable<StrokeFieldValue> Convert(AlienField fieldSource, EurekaField fieldTarget, string rawValue)
        {
            List<StrokeFieldValue> list = new List<StrokeFieldValue>();
            string value = PrepareValue(rawValue, fieldSource, fieldTarget);

            if (!string.IsNullOrEmpty(rawValue))
            {
                string[] valueItems = rawValue.Split(':');
                if (ctx == null)
                    ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
                foreach (string valueItem in valueItems)
                {
                    var val = GetInitialValue(fieldSource,fieldTarget,valueItem);
                    parseValue(val, fieldSource, fieldTarget);

                    //ctx.AddToStrokeFieldValue(val);
                    val.EurekaFieldReference.Value = fieldTarget;
                    list.Add(val);
                }
            }

            StrokeFieldValue[] strokeFields = list.ToArray();
            return strokeFields;
        }
    }
}
