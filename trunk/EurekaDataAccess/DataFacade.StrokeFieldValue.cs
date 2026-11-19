using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;

namespace Eureka.Stroke.Data
{
    public static partial class DataFacade
    {
        /// <summary>
        /// Get UploadLog records
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static StrokeFieldValue GetStrokeFieldValue(StrokeData data, string eurekaFieldName)
        {
            if (data.EntityState != System.Data.EntityState.Added)
                data.StrokeFieldValue.EnsureLoad();
            var field = (from f in data.StrokeFieldValue
                      let e = f.EurekaField
                      where e.Name == eurekaFieldName
                      select f).FirstOrDefault();

            
            return field;
        }

        public static int? GetStrokeFieldValueInt(StrokeData data, string eurekaFieldName)
        {
            var field = GetStrokeFieldValue(data, eurekaFieldName);
            if (field!=null)
                return field.IntegerValue;
            else return null;
        }

        public static bool? GetStrokeFieldValueBool(StrokeData data, string eurekaFieldName)
        {
            var field = GetStrokeFieldValue(data, eurekaFieldName);
            if (field != null)
            return field.BooleanValue;
            else return null;
        }

        public static DateTime? GetStrokeFieldValueDate(StrokeData data, string eurekaFieldName)
        {
            var field = GetStrokeFieldValue(data, eurekaFieldName);
            if (field != null)
            return field.DateTimeValue;
            else return null;
        }

        public static decimal? GetStrokeFieldValueDecimal(StrokeData data, string eurekaFieldName)
        {
            var field = GetStrokeFieldValue(data, eurekaFieldName);
            if (field != null)
            return field.DecimalValue;
            else return null;
        }

        public static string GetStrokeFieldValueString(StrokeData data, string eurekaFieldName)
        {
            var field = GetStrokeFieldValue(data, eurekaFieldName);
            if (field != null)
                return field.StringValue;
            else return string.Empty;
        }


        //public static StrokePivotData GetPivotStrokeData()
        //{
        //    var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
        //    StrokePivotData data = ctx.GetPivotStrokeData();
        //    return data;
        //}

        


    }
}

