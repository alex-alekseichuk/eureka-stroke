using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.Data;

namespace Eureka.Stroke.Converter
{
    /// <summary>
    /// Abstract converter class
    /// </summary>
    public abstract class BaseConverter : IConverter
    {
        protected EurekaStrokeEntities ctx = null;

        public BaseConverter(EurekaStrokeEntities ctx)
        {
            this.ctx = ctx;
            Container.DIContainer.RegisterType<IValueConverter, StringValueConverter>(EnumFieldType.String.ToString());
            Container.DIContainer.RegisterType<IValueConverter, IntegerValueConverter>(EnumFieldType.Integer.ToString());
            Container.DIContainer.RegisterType<IValueConverter, BooleanValueConverter>(EnumFieldType.Boolean.ToString());
            Container.DIContainer.RegisterType<IValueConverter, DecimalValueConverter>(EnumFieldType.Decimal.ToString());

            Container.DIContainer.RegisterType<IValueConverter, DateTimeValueConverter>(EnumFieldType.DateTime.ToString());
            Container.DIContainer.RegisterType<IValueConverter, SingleSelectValueConverter>(EnumFieldType.SingleSelect.ToString());
            Container.DIContainer.RegisterType<IValueConverter, MultipleSelectValueConverter>(EnumFieldType.MultipleSelect.ToString());

        }

        #region IConverter Members

        public virtual ConverterResponse ConvertValue(ConverterRequest req)
        {

            throw new NotImplementedException("This is abstract class, the implementation should be in derived one");
        }

        #endregion

        /// <summary>
        /// Get source field definition
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual AlienField GetSourceField(string fieldname, Format format)
        {
            AlienField def = AlienFieldDefinitionCache.Instance[format, fieldname];
            return def;
        }

        
        /// <summary>
        /// Get target field definition
        /// </summary>
        /// <param name="sourceField"></param>
        /// <returns></returns>
        protected virtual EurekaField GetTargetField(AlienField sourceField)
        {
            EurekaField targetField = null;
            if (sourceField != null)
            {
                sourceField.EurekaFieldReference.EnsureLoad();
                targetField = sourceField.EurekaField;
            }

            return targetField;
        }


    }
}
