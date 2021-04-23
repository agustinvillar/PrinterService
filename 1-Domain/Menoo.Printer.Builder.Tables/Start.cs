using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.Printer.Builder.Tables.Repository;
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
            var fireStore = dependencyResolver.Resolve<FirestoreDb>();
            dependencyResolver.Register(() => {
                var tableOpeningRepository = new TableOpeningFamilyRepository(fireStore);
                return tableOpeningRepository;
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
