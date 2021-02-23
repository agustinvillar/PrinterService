using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Extensions;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders.Repository
{
    public sealed class OrderRepository : FirebaseRepository<OrderV2>
    {
        private readonly FirestoreDb _firebaseDb;

        public OrderRepository(FirestoreDb firebaseDb)
            : base(firebaseDb)
        {
            _firebaseDb = firebaseDb;
        }

        public async Task<OrderV2> GetOrderById(string documentId)
        {
            var orderDTO = await base.GetById<OrderV2>(documentId, "orders");
            string orderJson = JsonConvert.SerializeObject(orderDTO);
            try
            {
                orderDTO.Items.ForEach(i =>
                {
                    i.MultiplePromotions = orderJson.GetPromotions<List<CategoryPromotions>>();
                });
            }
            catch
            {
                orderDTO.Items.ForEach(i =>
                {
                    i.SinglePromotions = orderJson.GetPromotions<CategoryPromotions>();
                });
            }
            orderDTO.Id = documentId;
            return orderDTO;
        }
    }
}
