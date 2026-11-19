using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    /// <summary>
    /// Regular converter implemenation
    /// </summary>
    public class Converter : BaseConverter
    {
        public Converter(EurekaStrokeEntities ctx)
            : base(ctx)
        {
        }


        #region IConverter Members

        /// <summary>
        /// Convert value
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override ConverterResponse ConvertValue(ConverterRequest req)
        {
            string msg = string.Empty;
            AlienField sourceField = GetSourceField(req.FieldName, req.Format);

            if (sourceField == null)
            {
                msg = string.Format("Field '{0}' is not known in format '{1}'", req.FieldName, req.Format.Name);
                ConverterResponse resp = new ConverterResponse(-1, msg, EnumErrorType.MissingField);
                return resp;
            }

            EurekaField targetField = GetTargetField(sourceField);
            if (targetField == null)
            {
                msg = string.Format("Mapping for Field '{0}' is not found from format '{1}' to format '{2}'", req.FieldName, req.Format.Name, "Eureka");
                ConverterResponse resp = new ConverterResponse(-1, msg, EnumErrorType.MissingFieldMapping);
                return resp;
            }

            if (targetField != null)
            {
                EnumFieldType fieldType = targetField.FieldTypeEnum;
                IValueConverter valueConverter = Container.DIContainer.Resolve<IValueConverter>(fieldType.ToString());
                IEnumerable<StrokeFieldValue> strokeFields = valueConverter.Convert(sourceField, targetField, req.FieldValue);

                EnumErrorType type = valueConverter.ErrorType;

                ConverterResponse resp = new ConverterResponse(valueConverter.ErrorCode, valueConverter.ErrorMessage, type)
                {
                    SourceFieldDefinition = sourceField,
                    TargetFieldDefinition = targetField,
                    StrokeValues = strokeFields
                };
                return resp;
            }

            return null;
        }

        #endregion

    }
}