using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Data
{
    /// <summary>
    /// Data access routines
    /// </summary>
    public static partial class DataFacade
    {
        /// <summary>
        /// Get current user
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static object GetCurrentUser()
        {

            return EurekaStrokeEntities.GetCurrentUser();
        }
    }
}
