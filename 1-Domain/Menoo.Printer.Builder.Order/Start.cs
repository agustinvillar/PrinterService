using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using System;

namespace Menoo.Printer.Builder.Orders
{
    [OnStartUp(Module = "Order.Builder", Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            var fireStore = dependencyResolver.Resolve<FirestoreDb>();
            dependencyResolver.Register(() => {
                var paymentRepository = new PaymentRepository(fireStore);
                return paymentRepository;
            });
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
