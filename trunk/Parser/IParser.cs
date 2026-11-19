using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Parser
{
    
    /// <summary>
    /// Interface to use parser for
    /// 1) run parsing in online mode and show parsing results right on the page
    /// 2) run parsing in offline mode (by windows service)
    /// </summary>
    public interface IParser : IError
    {
        /// <summary>
        /// Run parsing
        /// </summary>
        /// <param name="mode">Online or Offline mode</param>
        /// <param name="uploadLog">Specified upload log entry to parse</param>
        /// <returns></returns>
        Result Parse(Mode mode, UploadLog uploadLog);
    }

}
