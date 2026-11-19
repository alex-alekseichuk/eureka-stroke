using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eureka.Stroke;
using Eureka.Stroke.FormatService;
using Microsoft.Practices.Unity;
using Eureka.Stroke.Parser;
using Eureka.Stroke.Converter;

namespace EurekaTest
{
    public static class Initializer
    {

        public static void BootStrap()
        {
            IUnityContainer container = DIContainer;

            EurekaStrokeEntities ctx = Utils.GetDataContext();
            container.RegisterInstance<EurekaStrokeEntities>(ctx);

            IConverter converter = new Eureka.Stroke.Converter.Converter(ctx);
            container.RegisterInstance<IConverter>(converter);

            IFormatService formatService = new Eureka.Stroke.FormatService.FormatService("EurekaTest");
            container.RegisterInstance<IFormatService>(formatService);

            var dalConstructor = new InjectionConstructor(
                        new ResolvedParameter<EurekaStrokeEntities>()
                        
                        );

            container.RegisterType<IParser, Parser>(dalConstructor);

        }

        public static IUnityContainer DIContainer
        {
            get
            {
                return Container.DIContainer;
            }
        }
    }
}
