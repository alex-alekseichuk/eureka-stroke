using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Eureka.Stroke.Converter;

namespace Eureka.Stroke.Parser
{
    /// <summary>
    /// Handle parser initialization code
    /// </summary>
    public static class Configurator
    {
        public static void BootStrap()
        {
            Container.BootStrap();
            
            IUnityContainer container = Container.DIContainer;
            var ctx = container.Resolve<EurekaStrokeEntities>();

            IConverter converter = new Eureka.Stroke.Converter.Converter(ctx);
            container.RegisterInstance<IConverter>(converter);

            var dalConstructor = new InjectionConstructor(
                        new ResolvedParameter<EurekaStrokeEntities>()

                        );
            container.RegisterType<IParser, Parser>(dalConstructor);
        }
    }
}
