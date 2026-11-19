using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Stub temporary implementation is used for testing purposes only.
    /// </summary>
    public class TestParser : BaseService, IParser
    {
        public Result Parse(Mode mode, UploadLog uploadLog)
        {
            // set the status of uploadLog as completed
            uploadLog.Status = 2;

            // save changes UploadLog to DB
            var ctx = Container.DIContainer.Resolve<EurekaStrokeEntities>();
            ctx.SaveChanges();

            // just returns done as it's pure test parser
            return Result.Done;
        }
    }
}
