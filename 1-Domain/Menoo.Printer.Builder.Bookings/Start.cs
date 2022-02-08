using Menoo.Backend.Integrations.Constants;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using System;

namespace Menoo.Printer.Builder.BookingBuilder
{
    [OnStartUp(Module = PrintBuilder.BOOKING_BUILDER, Order = 2)]
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
