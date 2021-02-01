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

        public static bool IsCancelledPrinted(this DocumentSnapshot snapshot)
        {
            bool isCancelled = false;
            if (IsExistsPropertyPrinted(snapshot, "orderCancelledPrinted"))
            {
                var document = snapshot.ToDictionary();
                isCancelled = bool.Parse(document["orderCancelledPrinted"].ToString());
            }
            return isCancelled;
        }

        public static bool IsCreatedPrinted(this DocumentSnapshot snapshot)
        {
            bool isCreated = false;
            if (IsExistsPropertyPrinted(snapshot, "orderCreatedPrinted"))
            {
                var document = snapshot.ToDictionary();
                isCreated = bool.Parse(document["orderCreatedPrinted"].ToString());
            }
            return isCreated;
        }

        public static bool IsExistsPropertyCancelledPrinted(this DocumentSnapshot snapshot)
        {
            return IsExistsPropertyPrinted(snapshot, "orderCancelledPrinted");
        }

        public static bool IsExistsPropertyCreatedPrinted(this DocumentSnapshot snapshot)
        {
            return IsExistsPropertyPrinted(snapshot, "orderCreatedPrinted");
        }

        private static bool IsExistsPropertyPrinted(DocumentSnapshot snapshot, string property)
        {
            try
            {
                var document = snapshot.ToDictionary();
                return document.ContainsKey(property);
            }
            catch
            {
                return false;
            }
        }
    }
}
