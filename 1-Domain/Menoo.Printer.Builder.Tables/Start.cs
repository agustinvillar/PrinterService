using Menoo.Backend.Integrations.Constants;
using Menoo.PrinterService.Infraestructure;
using System;

namespace Menoo.Printer.Builder.Tables
{
    [OnStartUp(Module = PrintBuilder.TABLE_BUILDER, Order = 3)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            var listeners = Utils.DiscoverListeners(this.GetType().Assembly);
            foreach (var tuple in listeners)
            {
                dependencyResolver.Register(tuple.Item1, tuple.Item2, Guid.NewGuid().ToString());
            }
        }
    }
}
