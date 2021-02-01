using Menoo.PrinterService.Infraestructure.Configuration;
using Menoo.PrinterService.Infraestructure.DI;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.PrinterService.Infraestructure
{
    public static class GlobalConfig
    {
        public static IDependencyResolver DependencyResolver = new UnityDependencyResolver(Boostrapper.UnityContainer);

        public static IConfigurationManager ConfigurationManager = new DefaultConfigurationManager();
    }
}
