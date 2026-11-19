using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke
{
    /// <summary>
    /// Parser may be runned in one of 2 modes:
    /// 1) Online to return result right after the parsing directly to page response
    /// 2) Offline 
    /// </summary>
    public enum Mode : int
    {
        Online = 0,
        Offline = 1
    }

    /// <summary>
    /// Result returned by Parse method.
    /// There error codes may be added here.
    /// </summary>
    public enum Result : int
    {
        /// <summary>
        /// Online parsing was done correctly. Or offline parsing was started successfully.
        /// </summary>
        Done = 0,

        /// <summary>
        /// Returned by Parse method in Online mode
        /// if the source data is too huge to parse in online mode.
        /// It means we need to parse it offline.
        /// </summary>
        NeedOffline = 1,

        /// <summary>
        /// Parsing could not start due to error
        /// </summary>
        Error = 2,

        /// <summary>
        /// In progress (executing now)
        /// </summary>
        InProgress = 3,

        /// <summary>
        /// Pending (will be executed later)
        /// </summary>
        //Pending = 4

    }

}
