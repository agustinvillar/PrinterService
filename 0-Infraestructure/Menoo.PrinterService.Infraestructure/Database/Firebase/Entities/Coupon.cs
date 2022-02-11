using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class OfferCoupon
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("code")]
        [JsonProperty("code")]
        public string Code { get; set; }

        [FirestoreProperty("startDate")]
        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [FirestoreProperty("validUntil")]
        [JsonProperty("validUntil")]
        public string ValidUntil { get; set; }

        [FirestoreProperty("quantity")]
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [FirestoreProperty("active")]
        [JsonProperty("active")]
        public bool? Active { get; set; }

        [FirestoreProperty("showApp")]
        [JsonProperty("showApp")]
        public bool? ShowApp { get; set; }

        [FirestoreProperty("stores")]
        [JsonProperty("stores")]
        public List<StoreInfo> StoreInfo { get; set; }

        [FirestoreProperty("discountPercent")]
        [JsonProperty("discountPercent")]
        public double? DiscountPercent { get; set; }

        [FirestoreProperty("discountNumber")]
        [JsonProperty("discountNumber")]
        public double? DiscountNumber { get; set; }

        [FirestoreProperty("type")]
        [JsonProperty("type")]
        public List<string> Type { get; set; }

        [FirestoreProperty("genericCoupon")]
        [JsonProperty("genericCoupon")]
        public bool? GenericCoupon { get; set; }

        [FirestoreProperty("used")]
        [JsonProperty("used")]
        public int? Used { get; set; }

        [FirestoreProperty("usedInStore")]
        [JsonProperty("usedInStore")]
        public int? UsedInStore { get; set; }
    }

    public class StoreInfo
    {
        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("storeName")]
        [JsonProperty("storeName")]
        public string StoreName { get; set; }
    }
}
