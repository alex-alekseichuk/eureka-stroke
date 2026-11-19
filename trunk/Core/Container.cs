using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace Eureka.Stroke
{
    /// <summary>
    /// Wrapper for DI Unity container
    /// </summary>
    public static class Container
    {
        private static readonly IUnityContainer container = new UnityContainer();

        public static IUnityContainer DIContainer
        {
            get
            {
                return container;
            }
        }

        /// <summary>
        /// Initialize DI container
        /// </summary>
        public static void BootStrap()
        {
            IUnityContainer container = Container.DIContainer;

            EurekaStrokeEntities ctx = Utils.GetDataContext();
            container.RegisterInstance<EurekaStrokeEntities>(ctx);

        }
    }
}
