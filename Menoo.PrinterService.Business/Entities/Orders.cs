using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Orders
    {
        [FirestoreProperty("address")]
        [JsonProperty("address")]
        public virtual string Address { get; set; }

        [FirestoreProperty("printed")]
        [JsonProperty("printed")]
        public virtual bool Printed { get; set; }

        [FirestoreProperty("items")]
        [JsonProperty("items")]
        public virtual Item[] Items { get; set; }

        [FirestoreProperty("madeAt")]
        [JsonProperty("madeAt")]
        public virtual string MadeAt { get; set; }

        [FirestoreProperty("orderDate")]
        [JsonProperty("orderDate")]
        public virtual string OrderDate { get; set; }

        [FirestoreProperty("orderType")]
        [JsonProperty("orderType")]
        public virtual string OrderType { get; set; }

        [FirestoreProperty("tableOpeningFamilyId")]
        [JsonProperty("tableOpeningFamilyId")]
        public virtual string TableOpeningFamilyId { get; set; }

        [FirestoreProperty("takeAwayHour")]
        [JsonProperty("takeAwayHour")]
        public virtual string TakeAwayHour { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public virtual string UserName { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public virtual string StoreId { get; set; }

        [FirestoreProperty("orderNumber")]
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [FirestoreProperty("bookingId")]
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        public virtual string Id { get; set; }

        private Store _store;

        public virtual Store Store
        {
            get => _store;
            set
            {
                _store = value;
                if (Items != null) 
                {
                    foreach (var item in Items)
                    {
                        item.Store = value;
                    }
                }
            }
        }

        public virtual bool IsTakeAway => OrderType.ToUpper() == "TAKEAWAY";
    }

    [FirestoreData]
    public class OrdersCancelled
    {
        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("store")]
        [JsonProperty("store")]
        public StoreV2 Store { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("tableOpeningFamilyId")]
        [JsonProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }

        [FirestoreProperty("address")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [FirestoreProperty("madeAt")]
        [JsonProperty("madeAt")]
        public string MadeAt { get; set; }

        [FirestoreProperty("cancelMotive")]
        [JsonProperty("cancelMotive")]
        public string CancelMotive { get; set; }

        [FirestoreProperty("items")]
        [JsonProperty("items")]
        public List<ItemV2> Items { get; set; }

        [FirestoreProperty("takeAwayHour")]
        [JsonProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }

        [FirestoreProperty("orderNumber")]
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [FirestoreProperty("orderType")]
        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [FirestoreProperty("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("orderDate")]
        [JsonProperty("orderDate")]
        public string OrderDate { get; set; }

        [FirestoreProperty("updatedAt")]
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [FirestoreProperty("tableOpeningId")]
        [JsonProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }

        [FirestoreProperty("cancelSource")]
        [JsonProperty("cancelSource")]
        public string CancelSource { get; set; }

        [FirestoreProperty("bookingId")]
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        [FirestoreProperty("status")]
        [JsonProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public string Total { get; set; }
    }

    public sealed class OrderQR
    {
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }
    }

    public static class OrderPrintedExtensions
    {
        public static bool IsPrinted(this DocumentSnapshot snapshot)
        {
            try
            {
                var document = snapshot.ToDictionary();
                return document.ContainsKey("printed");
            }
            catch
            {
                return false;
            }
        }

        public static OrdersCancelled GetOrderData(this DocumentSnapshot snapshot)
        {
            var document = snapshot.ToDictionary();
            var objectCasted = Utils.GetObject<OrdersCancelled>(document);
            return objectCasted;
        }
    }
}
