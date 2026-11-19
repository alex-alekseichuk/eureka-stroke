using System;
using System.Configuration;
using System.Reflection;
using System.Web;

using Eureka.Stroke;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity;
using Eureka.Stroke.Converter;
using Eureka.Stroke.Parser;

//namespace Eureka.Stroke.Web
//{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            lock (this)
            {
                var container = Container.DIContainer;
                var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
                section.Containers.Default.Configure(container);

                BootStrap();

                Logger.Instance.LogInfo("Application started.");
            }
        }

        /// <summary>
        /// Initialize DI container
        /// </summary>
        public static void BootStrap()
        {
            Configurator.BootStrap();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }


        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            HttpRequest request = null;
            try
            {
                request = HttpContext.Current.Request;
            }
            catch
            { }
            var message =
                request == null
                ? "Error in type:" + sender.GetType().FullName
                : "Error in path:" + request.RawUrl; //Get the path of the page
            var ex = Server.GetLastError();
            Logger.Instance.LogExceptionRecursively(message, ex, MethodBase.GetCurrentMethod());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.Instance.LogInfo("Application ended.");
        }


    }
//}
