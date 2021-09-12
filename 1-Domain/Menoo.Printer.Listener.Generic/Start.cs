using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Interceptors;
using System;

namespace Menoo.Printer.Listener.Generic
{
    [OnStartUp(Module = "Generic.Listener", Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            var context = dependencyResolver.Resolve<PrinterContext>();
            var interceptor = new OnActionRecieve(context);
            dependencyResolver.Register(() => { return interceptor; });
            var listeners = Utils.DiscoverListeners(this.GetType().Assembly);
            foreach (var tuple in listeners)
            {
                dependencyResolver.Register(tuple.Item1, tuple.Item2, Guid.NewGuid().ToString());
            }
        }
    }
}
