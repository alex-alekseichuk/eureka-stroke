using System;
using System.Collections;
using System.Reflection;
using System.Text;
using NLog;

namespace Eureka.Stroke
{
    ///<summary>
    /// Standard logger for .Net 3.5 projects.
    /// NLog-based.
    ///</summary>
    public class Logger
    {
        public static readonly Logger Instance = new Logger();

        private readonly NLog.Logger currentLogger;

        private Logger()
        {
            currentLogger = GetNLogLoggerByName("Logger");
        }

        ///<summary>
        /// Get underlying NLog logger
        ///</summary>
        ///<returns></returns>
        public NLog.Logger GetNlogLogger()
        {
            return currentLogger;
        }

        ///<summary>
        /// Wrapper for NLog.LogManager.GetLogger
        ///</summary>
        ///<param name="name"></param>
        ///<returns></returns>
        public static NLog.Logger GetNLogLoggerByName(string name)
        {
            return LogManager.GetLogger(name);
        }

        ///<summary>
        /// Returns exception details with all inner exceptions included
        ///</summary>
        ///<param name="ex"></param>
        ///<param name="headerText"></param>
        ///<returns>StringBuilder with the requested details</returns>
        public static StringBuilder GetExceptionDetailsRecursively(Exception ex, string headerText)
        {
            var builder = new StringBuilder().AppendLine().AppendLine(headerText);
            do
            {
                try
                {
                    builder.AppendFormat("Type: {0}", ex.GetType().FullName).AppendLine();
                    builder.AppendFormat("Message: {0}", ex.Message).AppendLine();
                    builder.AppendFormat("Source: {0}", ex.Source).AppendLine();
                    builder.AppendFormat("HelpLink: {0}", ex.HelpLink).AppendLine();
                    builder.AppendFormat("TargetSite: {0}", ex.TargetSite).AppendLine();
                    builder.AppendFormat("Data (Count = {0}):", ex.Data.Count).AppendLine();
                    foreach (DictionaryEntry dictionaryEntry in ex.Data)
                    {
                        builder.AppendFormat("    [{0}] = '{1}'", dictionaryEntry.Key, dictionaryEntry.Value).AppendLine
                            ();
                    }
                    builder.AppendFormat("StackTrace: {0}", ex.StackTrace).AppendLine();

                    ex = ex.InnerException;
                    if (ex != null)
                    {
                        builder.AppendLine().AppendLine("Inner Exception: ");
                    }
                }
                catch
                {
                    builder.AppendLine().AppendLine("Error getting exception details: ");
                    ex = null;
                }
            } while (null != ex);
            builder.AppendLine("===========================").AppendLine();
            return builder;
        }


        /// <summary>
        /// Delegate for assertion condition
        /// </summary>
        /// <returns></returns>
        public delegate bool ConditionDelegate();

        /// <summary>
        /// Documenting assert by condition delegate
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="detailedMessage"></param>
        public void Assert(ConditionDelegate condition, string message, string detailedMessage)
        {
            Assert(condition(), message, detailedMessage);
        }

        /// <summary>
        /// Documenting assert by condition
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="detailedMessage"></param>
        public void Assert(bool condition, string message, string detailedMessage)
        {
            if (!condition)
            {
                LogMessage(message, detailedMessage);
            }
        }

        /// <summary>
        /// Logging message to Info log with header
        /// </summary>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        public void LogMessage(string msg1, string msg2)
        {
            string message = string.Format("[{0}] {1}", msg1, msg2);
            LogMessage(message);
        }

        /// <summary>
        /// Logging message to Info log
        /// </summary>
        /// <param name="msg"></param>
        public void LogMessage(string msg)
        {
            LogInfo(msg);
        }

        /// <summary>
        /// Logging message to Info log
        /// </summary>
        /// <param name="msg"></param>
        public void LogInfo(string msg)
        {
            GetNlogLogger().Info(msg);
        }
        /// <summary>
        /// Logging message to Debug log
        /// </summary>
        /// <param name="msg"></param>
        public void LogDebug(string msg)
        {
            GetNlogLogger().Debug(msg);
        }

        /// <summary>
        /// Standard logging exception to Error log
        /// </summary>
        /// <param name="exception"></param>
        public void LogException(Exception exception)
        {
            LogException(string.Empty, exception);
        }

        /// <summary>
        /// Standard logging exception to Error log
        /// </summary>
        /// <param name="headerText"></param>
        /// <param name="exception"></param>
        public void LogException(string headerText, Exception exception)
        {
            //GetNlogLogger().Error(GetExceptionDetailsRecursively(exception, headerText).ToString());
            GetNlogLogger().ErrorException(headerText, exception);
        }

        /// <summary>
        /// Logging exception recursively to Error log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <param name="catchingMethod"></param>
        public void LogExceptionRecursively(string message, Exception exception, MethodBase catchingMethod)
        {
            GetNlogLogger().Error(string.Format("Error in method: {0}\n{1}", catchingMethod.Name,
                                                GetExceptionDetailsRecursively(exception, message)));
        }

        public void LogActionException(string message, Exception exception)
        {
            GetNlogLogger().Error(string.Format("Error in {0}", GetExceptionDetailsRecursively(exception, message)));
        }

    }
}