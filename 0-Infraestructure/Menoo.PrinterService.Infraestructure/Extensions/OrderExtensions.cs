using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class OrderExtensions
    {
        public static OrderV2 GetOrderData(this DocumentSnapshot snapshot)
        {
            var document = snapshot.ToDictionary();
            var objectCasted = Utils.GetObject<OrderV2>(document);
            string orderJson = JsonConvert.SerializeObject(objectCasted);
            try
            {
                objectCasted.Items.ForEach(i =>
                {
                    i.MultiplePromotions = orderJson.GetPromotions<List<CategoryPromotions>>();
                });
            }
            catch
            {
                objectCasted.Items.ForEach(i =>
                {
                    i.SinglePromotions = orderJson.GetPromotions<CategoryPromotions>();
                });
            }
            objectCasted.Id = snapshot.Id;
            return objectCasted;
        }
    }
}
