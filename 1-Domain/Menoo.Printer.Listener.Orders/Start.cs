using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.Printer.Listener.Orders
{
    [OnStartUp(Module = "Order.Listener", Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register<IFirebaseListener, OrderListener>();
        }
    }
}
