using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Runtime.Serialization.Json;
using System.Web;
using System.IO;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;
using System.Data;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;
using System.Data.SqlClient;

namespace Eureka.Stroke
{
    public static class Utils
    {
        #region Primitive types converstions

        public static short ToShort(this object o)
        {
            try
            {
                return Convert.ToInt16(o);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int ToInt(this object o)
        {
            try
            {
                return Convert.ToInt32(o);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static long ToLong(this object o)
        {
            try
            {
                return Convert.ToInt64(o);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// Return a number of lines in a reader unless max number is not reached
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static long GetNumberOfLines(StreamReader reader, int maxCount)
        {
            long lineCount = 0;
            while (reader.ReadLine() != null && lineCount<=maxCount)
            {
                lineCount++;
            }
            return lineCount;
        }

        /// <summary>
        /// Get Data Context instance
        /// </summary>
        /// <returns></returns>
        public static EurekaStrokeEntities GetDataContext()
        {
            string dbConnString = Config.Instance.ConnectionStringName;
            EurekaStrokeEntities ctx = null;
            if (string.IsNullOrEmpty(dbConnString))
                ctx = new EurekaStrokeEntities();
            else
            {
                string connString = Config.GetConnectionString(dbConnString);
                ctx = new EurekaStrokeEntities(connString);
            }
            return ctx;
        }


        /// <summary>
        /// Truncate the string for specified length to be used in web interface
        /// </summary>
        /// <param name="source"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string Truncate(string source, int max)
        {
            if (null == source)
                return "";
            if (source.Length > max)
                return source.Substring(0, max) + "...";
            return source;
        }

        public static DateTime GetDateMinValue()
        {
            DateTime d = new DateTime(1900, 1, 1);
            return d;
        }

    }


    public static class Serialize
    {
        public static string ToJSON(this Object obj)
        {
            using (var buffer = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(obj.GetType());
                ser.WriteObject(buffer, obj);
                buffer.Seek(0, SeekOrigin.Begin);
                return new StreamReader(buffer).ReadToEnd();
            }
        }
    }

    



    /// <summary>
    /// Linq extensions class
    /// </summary>
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Clone entity object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static EntityObject CloneEntity(this EntityObject obj)
        {
            DataContractSerializer dcSer = new DataContractSerializer(obj.GetType());
            MemoryStream memoryStream = new MemoryStream();

            dcSer.WriteObject(memoryStream, obj);
            memoryStream.Position = 0;

            EntityObject newObject = (EntityObject)dcSer.ReadObject(memoryStream);
            return newObject;
        }

        /// <summary>
        /// Ensures that entity referenced by <paramref name="entityReference"/> is loaded.
        /// </summary>
        public static void EnsureLoad<T>(this EntityReference<T> entityReference) where T : EntityObject
        {
            if (!entityReference.IsLoaded)
            {
                entityReference.Load();
            }
        }

        /// <summary>
        /// Ensures that entity collection is loaded.
        /// </summary>
        public static void EnsureLoad<T>(this EntityCollection<T> entityCollection) where T : EntityObject
        {
            if (!entityCollection.IsLoaded)
            {
                entityCollection.Load();
            }
        }

        /// <summary>
        /// Combined method to get value from dictionary or add if missing
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TDictionaryValue"></typeparam>
        /// <typeparam name="TActualValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static TActualValue GetOrAdd<TKey, TDictionaryValue, TActualValue>(this IDictionary<TKey, TDictionaryValue> dictionary, TKey key, Func<TActualValue> newValue) where TActualValue : TDictionaryValue
        {
            TDictionaryValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = newValue.Invoke();
                dictionary.Add(key, value);
            }
            return (TActualValue)value;
        }


        

        /// <summary>
        /// Creates a store command based on the connection used by given object context.
        /// </summary>
        /// <param name="context">Object context.</param>
        /// <param name="commandText">Command text.</param>
        /// <param name="parameters">Parameters to pass to the store command.</param>
        /// <returns>Store command.</returns>
        public static DbCommand CreateStoreCommand(this ObjectContext context, string commandText, params object[] parameters)
        {
            return CreateStoreCommand(context, commandText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Creates a store command based on the connection used by given object context.
        /// </summary>
        /// <param name="context">Object context.</param>
        /// <param name="commandText">Command text.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="parameters">Parameters to pass to the store command.</param>
        /// <returns>Store command.</returns>
        public static DbCommand CreateStoreCommand(this ObjectContext context, string commandText, CommandType commandType, params object[] parameters)
        {

            context.CommandTimeout = 10 * 60;
            EntityConnection entityConnection = (EntityConnection)context.Connection;
            DbConnection storeConnection = entityConnection.StoreConnection;
            DbCommand storeCommand = storeConnection.CreateCommand();

            // setup command
            storeCommand.CommandText = commandText;
            storeCommand.CommandType = commandType;
            if (null != parameters)
            {
                storeCommand.Parameters.AddRange(parameters);
            }

            // pass through command timeout as appropriate
            if (context.CommandTimeout.HasValue)
            {
                storeCommand.CommandTimeout = context.CommandTimeout.Value;
            }

            return storeCommand;
        }

        /// <summary>
        /// Return Ado.Net SqlConnection from EntityConnection
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static SqlConnection GetSqlConnection(this ObjectContext ctx)
        {
            EntityConnection ec = (EntityConnection)ctx.Connection;
            SqlConnection sc = (SqlConnection)ec.StoreConnection;
            return sc;
        }


    }

}
