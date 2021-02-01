using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.Printer.Listener.Tables
{
    [OnStartUp(Module = "Tables.Listener", Order = 3)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register<IFirebaseListener, TablesListener>();
        }
    }
}
