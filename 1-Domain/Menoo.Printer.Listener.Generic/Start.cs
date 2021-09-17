using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Repository;
using System;

namespace Menoo.Printer.Listener.Generic
{
    [OnStartUp(Module = "Generic.Listener", Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            var repository = dependencyResolver.Resolve<TicketRepository>();
            var interceptor = new OnActionRecieve(repository);
            dependencyResolver.Register(() => { return interceptor; });
            var listeners = Utils.DiscoverListeners(this.GetType().Assembly);
            foreach (var tuple in listeners)
            {
                dependencyResolver.Register(tuple.Item1, tuple.Item2, Guid.NewGuid().ToString());
            }
        }
    }
}
