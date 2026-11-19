using System;
using System.Collections.Generic;
using System.ComponentModel;
using Eureka.Stroke;
using Eureka.Stroke.FormatService;

namespace WebApplication
{

    public class FormatManagerDataSource
    {
        #region Formats

        public IList<Format> GetFormats()
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            var ret = formatService.GetFormats();

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);

            return ret;
        }

        #endregion

        #region eurika fields

        public IList<EurekaField> GetEurekaFields(int formatID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var format = formatService.GetFormatByID(formatID);
            if (null == format)
                return null;

            var ret = formatService.GetEurekaFields(format);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);

            return ret;
        }
        public IDictionary<int, string> GetFieldTypes()
        {
            return FieldType.Types;
        }


        public void UpdateEurekaField(int ID, short FieldType, string Name, string Description)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            formatService.UpdateEurekaField(ID, FieldType, Name, Description);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }


        public void DeleteEurekaField(int ID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            var field = formatService.GetEurekaFieldByID(ID);
            if (null != field)
                formatService.DeleteEurekaField(field);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }

        public void InsertEurekaField(short Type, string Name, string Description, out int rowIndex)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            var field = new EurekaField { ID = 0, FieldType = Type, Name = Name, Description = Description };
            rowIndex = formatService.InsertEurekaField(field);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }
        #endregion

        #region alien fields

        public IList<AlienField> GetAlienFields(int eurekaFieldID, int formatID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var format = formatService.GetFormatByID(formatID);
            if (null == format)
                return null;

            var eurekaField = formatService.GetEurekaFieldByID(eurekaFieldID);
            if (null == eurekaField)
                return null;

            var ret = formatService.GetAlienFields(eurekaField, format);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);

            return ret;
        }

        public void DeleteAlienField(int ID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var alienField = formatService.GetAlienFieldByID(ID);
            if (null != alienField)
                formatService.DeleteAlienField(alienField);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }

        public void UpdateAlienField(int ID, string Name, string RegularExpression)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            formatService.UpdateAlienField(ID, Name, RegularExpression);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }

        public void InsertAlienField(int eurekaFieldID, int formatID, string Name, string RegularExpression)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            var alienField = new AlienField { ID = 0, Name = Name, RegularExpression = RegularExpression };
            formatService.InsertAlienField(eurekaFieldID, formatID, alienField);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }

        #endregion

        #region values

        public IList<ValueMap> GetEurekaValues(int eurekaFieldID, int formatID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var format = formatService.GetFormatByID(formatID);
            if (null == format)
                return null;

            var eurekaField = formatService.GetEurekaFieldByID(eurekaFieldID);
            if (null == eurekaField)
                return null;

            var ret = formatService.GetValuesMap(eurekaField, format);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);

            return ret;
        }

        public void DeleteEurekaValue(int eurekaID, int alienID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var valueMap = formatService.GetValueMap(eurekaID, alienID);
            if (null != valueMap)
                formatService.DeleteValueMap(valueMap);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }

        public void UpdateEurekaValue(int eurekaID, int alienID, int eurekaFieldID, int formatID, string eurekaValue, string alienValue, string eurekaDescription, string alienDescription)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var valueMap = formatService.GetValueMap(eurekaID, alienID);
            valueMap.EurekaValue = eurekaValue;
            valueMap.AlienValue = alienValue;
            valueMap.EurekaDescription = eurekaDescription;
            valueMap.AlienDescription = alienDescription;
            formatService.SaveValueMap(eurekaFieldID, formatID, valueMap);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }
        public void InsertEurekaValue(int eurekaFieldID, int formatID, string EurekaValue, string AlienValue, string EurekaDescription, string AlienDescription)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

            var value = new ValueMap { EurekaValue = EurekaValue, AlienValue = AlienValue, EurekaDescription = EurekaDescription, AlienDescription = AlienDescription };
            formatService.SaveValueMap(eurekaFieldID, formatID, value);

            if (0 != formatService.ErrorCode)
                throw new Exception(formatService.ErrorMessage);
        }
        #endregion

    }
}