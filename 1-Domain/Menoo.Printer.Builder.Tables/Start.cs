using Menoo.PrinterService.Infraestructure;
using System;

namespace Menoo.Printer.Builder.Tables
{
    [OnStartUp(Module = "Tables.Builder", Order = 3)]
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
