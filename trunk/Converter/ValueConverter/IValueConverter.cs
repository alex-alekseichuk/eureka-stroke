using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Converter
{
    /// <summary>
    /// raw value converter
    /// </summary>
    public interface IValueConverter : IError
    {
        IEnumerable<StrokeFieldValue> Convert(AlienField fieldSource, EurekaField fieldTarget, string rawValue);

        bool IsValid
        {
            get;
        }

        /// <summary>
        /// Error type
        /// </summary>
        EnumErrorType ErrorType { get; set; }

        /// <summary>
        /// Error processing policy
        /// </summary>
        EnumErrorProcessingType ErrorProcessingType { get; }
    }
}
