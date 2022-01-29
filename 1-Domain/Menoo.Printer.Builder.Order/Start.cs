using Google.Cloud.Firestore;
using Menoo.Backend.Integrations.Constants;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema;
using System;

namespace Menoo.Printer.Builder.Orders
{
    [OnStartUp(Module = PrintBuilder.ORDER_BUILDER, Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            var fireStore = dependencyResolver.Resolve<FirestoreDb>();
            dependencyResolver.RegisterPerThread(() => { return new MenooContext(); });
            dependencyResolver.Register(() => {
                var orderRepository = new OrderRepository(fireStore);
                return orderRepository;
            });
            var listeners = Utils.DiscoverListeners(this.GetType().Assembly);
            foreach (var tuple in listeners)
            {
                dependencyResolver.Register(tuple.Item1, tuple.Item2, Guid.NewGuid().ToString());
            }
        }
    }
}
