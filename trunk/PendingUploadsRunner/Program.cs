using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke.Parser;
using Eureka.Stroke.Data;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;

namespace Eureka.Stroke.PendingUploadsRunner
{
    /// <summary>
    /// Run Pending Uploads Runner
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            // initialize types and classes
            BootStrap();

            IUnityContainer container = Container.DIContainer;
            EurekaStrokeEntities ctx = container.Resolve<EurekaStrokeEntities>();
            var pendingUploads = DataFacade.GetPendingUploadLogs(ctx);

            if (pendingUploads != null)
            {
                int cnt = pendingUploads.Count;
                msg = string.Format("PendingUploadsRunner: {0} pending upload entries are found", cnt);
                Logger.Instance.LogDebug(msg);
                Debug.Print(msg);
                int i = 1;
                foreach (var log in pendingUploads)
                {
                    msg = string.Format("#{0} Start processing file {1} ", i, log.OriginalFilename);
                    Logger.Instance.LogDebug(msg);
                    Debug.Print(msg);

                    // create a new parser instance
                    IParser parser = Container.DIContainer.Resolve<IParser>();

                    // run parser process in offline mode
                    Result res = parser.Parse(Mode.Offline, log);
                    int code = parser.ErrorCode;
                    string errorMessage = parser.ErrorMessage;

                    msg = string.Format("#{0} Finish processing file {1}, result {2}/{3}, message {4} ", i, log.OriginalFilename, res.ToString(), code, errorMessage);
                    Logger.Instance.LogDebug(msg);
                    Debug.Print(msg);
                    i++;
                }
            }


        }

        public static void BootStrap()
        {
            var container = Container.DIContainer;
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Containers.Default.Configure(container);

            Configurator.BootStrap();
        }
    }
}
