using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    public sealed class OrderQR
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("storeId")]
        public string StoreId { get; set; }
    }

    [FirestoreData]
    public class Order
    {
        [FirestoreProperty("address")]
        [JsonProperty("address")]
        public string Address { get; set; }

        [FirestoreProperty("bookingId")]
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        [FirestoreProperty("cancelMotive")]
        [JsonProperty("cancelMotive")]
        public string CancelMotive { get; set; }

        [FirestoreProperty("cancelSource")]
        [JsonProperty("cancelSource")]
        public string CancelSource { get; set; }

        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        public virtual string Id { get; set; }

        [FirestoreProperty("items")]
        [JsonProperty("items")]
        public List<ItemOrder> Items { get; set; }

        [FirestoreProperty("madeAt")]
        [JsonProperty("madeAt")]
        public string MadeAt { get; set; }

        [FirestoreProperty("orderDate")]
        [JsonProperty("orderDate")]
        public string OrderDate { get; set; }

        [FirestoreProperty("orderNumber")]
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [FirestoreProperty("orderType")]
        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [FirestoreProperty("status")]
        [JsonProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("store")]
        [JsonProperty("store")]
        public Store Store { get; set; }

        [FirestoreProperty("tableOpeningId")]
        [JsonProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }

        [FirestoreProperty("takeAwayHour")]
        [JsonProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public string Total { get; set; }

        [FirestoreProperty("updatedAt")]
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [FirestoreProperty("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("isMarket")]
        [JsonProperty("isMarket")]
        public bool IsMarket { get; set; }
    }
}
