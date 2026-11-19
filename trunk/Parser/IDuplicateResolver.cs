using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Parser
{
    public interface IDuplicateResolver : IError
    {
        /// <summary>
        /// Run parsing
        /// </summary>
        /// <param name="mode">Online or Offline mode</param>
        /// <param name="uploadLog">Specified upload log entry to parse</param>
        /// <returns></returns>
        StrokeData Resolve(StrokeData data, UploadLog log);

        EnumTemporalOverlapResult TemporalOverlapResult { get; }
    }
}
