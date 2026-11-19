using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Mockup duplicate resolver
    /// (Just returns the same instance)
    /// </summary>
    public class DuplicateResolverMockup : IDuplicateResolver
    {
        #region IDuplicateResolver Members

        public StrokeData Resolve(StrokeData data, UploadLog log)
        {
            return data;
        }

        #endregion

        #region IError Members

        public int ErrorCode
        {
            get { return 0; }
        }

        public string ErrorMessage
        {
            get { return string.Empty; }
        }

        #endregion

        #region IDuplicateResolver Members


        public EnumTemporalOverlapResult TemporalOverlapResult
        {
            get
            {
                return EnumTemporalOverlapResult.NewRecord;
            }
            
        }

        #endregion
    }
}
