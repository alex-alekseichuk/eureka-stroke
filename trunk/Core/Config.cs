using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using System.Web;

namespace Eureka.Stroke
{
    public class Config
    {
        private static Config _Instance = null;
        public static Config Instance
        {
            get
            {
                if (null == _Instance)
                    _Instance = new Config();
                return _Instance;
            }
        }

        /// <summary>
        /// path to folder where web uploader save raw CSV's
        /// </summary>
        public string UploaderFolder
        {

            get
            {
                if (string.IsNullOrEmpty(_UploaderFolder))
                    _UploaderFolder = GetConfigValue("UploaderFolder");
                return _UploaderFolder;
            }
        }
        private string _UploaderFolder = string.Empty;

        /// <summary>
        /// path to folder where web custom report manager save new reports
        /// </summary>
        public string ReportsFolder
        {

            get
            {
                if (string.IsNullOrEmpty(_ReportsFolder))
                    _ReportsFolder = GetConfigValue("ReportsFolder");
                return _ReportsFolder;
            }
        }
        private string _ReportsFolder = string.Empty;

        /// <summary>
        /// URL of reporting services without end slash
        /// </summary>
        public string ReportsServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_ReportsServerUrl))
                    _ReportsServerUrl = GetConfigValue("ReportServerUrl");
                return _ReportsServerUrl;
            }
        }
        private string _ReportsServerUrl = string.Empty;

        /// <summary>
        /// Path to the folder in reporting services
        /// </summary>
        public string ReportPath
        {
            get
            {
                if (string.IsNullOrEmpty(_ReportPath))
                    _ReportPath = GetConfigValue("ReportPath");
                return _ReportPath;
            }
        }
        private string _ReportPath = string.Empty;

        /// <summary>
        /// Name of the EF connection string
        /// </summary>
        public string ConnectionStringName
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionStringName))
                    _ConnectionStringName = Core.Properties.Settings.Default.EntityConnectionStringName;              
                return _ConnectionStringName;
            }
        }
        private string _ConnectionStringName = string.Empty;


        #region methods to read config values
        public static string GetConfigValue(string key, string defaultKeyValue)
        {
            var value = GetConfigValue(key);
            return value ?? defaultKeyValue;
        }
        public static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        public static string GetConnectionString(string key)
        {
            var str = ConfigurationManager.ConnectionStrings[key];
            return str.ConnectionString;
        }

        #endregion

    }
}
