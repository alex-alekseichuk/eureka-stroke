using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.FormatService
{

    /// <summary>
    /// Service layer Value mapping
    /// </summary>
    public class ValueMap
    {
        public int EurekaID { get; set; }
        public int AlienID { get; set; }
        public string EurekaValue { get; set; }
        public string AlienValue { get; set; }
        public string EurekaDescription { get; set; }
        public string AlienDescription { get; set; }
    }

}
