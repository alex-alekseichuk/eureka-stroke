using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    /// <summary>
    /// Special interface is used in services to provide error/status of the operations.
    /// After each operation of the service ErrorCode should be 0 if it's ok
    /// and non-null if there is some error occurred.
    /// ErrorCode means the numeric code of error for specified action.
    /// ErrorMessage is the string in english to be used to explain the error to user.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Error code of last FormatService operation or 0 if there was no error.
        /// </summary>
        int ErrorCode { get; }

        /// <summary>
        /// Error string maybe used as error message in the log.
        /// </summary>
        string ErrorMessage { get; }
    }

    public class BaseService : IError
    {
        /// <summary>
        /// Error message returned to the caller
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Error code returned to the caller
        /// </summary>
        public int ErrorCode { get; protected set; }

        /// <summary>
        /// Get error processing policy
        /// </summary>
        /// <param name="errorType"></param>
        /// <returns></returns>
        public static EnumErrorProcessingType GetErrorProcessingType(EnumErrorType errorType)
        {

            EnumErrorProcessingType type = EnumErrorProcessingType.Ignore;
            switch (errorType)
            {
                case EnumErrorType.None:
                case EnumErrorType.Conversion:
                case EnumErrorType.MissingValue:
                case EnumErrorType.MissingValueMapping:
                case EnumErrorType.Other:
                case EnumErrorType.Validation:
                    type = EnumErrorProcessingType.Capture;
                    break;

                case EnumErrorType.MissingField:
                case EnumErrorType.MissingFieldMapping:
                    type = EnumErrorProcessingType.Ignore;
                    break;
            }
            return type;

        }

    }
}
