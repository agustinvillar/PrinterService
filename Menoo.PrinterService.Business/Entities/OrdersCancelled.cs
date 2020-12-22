using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business.Entities
{
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

    [FirestoreData]
    public class ItemV2
    {
        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("storeId")]
        public string Total { get; set; }

        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("promotions")]
        [JsonProperty("promotions")]
        public Promotions Promotions { get; set; }

        [FirestoreProperty("priceWithDiscountTA")]
        [JsonProperty("priceWithDiscountTA")]
        public string PriceWithDiscountTA { get; set; }

        [FirestoreProperty("priceWithDiscount")]
        [JsonProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; }

        [FirestoreProperty("quantity")]
        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [FirestoreProperty("priceTA")]
        [JsonProperty("priceTA")]
        public string PriceTA { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public string Price { get; set; }

        [FirestoreProperty("categoryId")]
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [FirestoreProperty("options")]
        [JsonProperty("options")]
        public ItemOptionV2[] Options { get; set; }

        [FirestoreProperty("subTotal")]
        [JsonProperty("subTotal")]
        public string SubTotal { get; set; }

        [FirestoreProperty("itemIsTA")]
        [JsonProperty("itemIsTA")]
        public string ItemIsTA { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("thumbnail")]
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("softId")]
        [JsonProperty("softId")]
        public string SoftId { get; set; }
    }

    [FirestoreData]
    public class StoreV2
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("logoImage")]
        [JsonProperty("logoImage")]
        public string LogoImage { get; set; }
    }

    [FirestoreData]
    public class ItemOptionV2
    {
        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public string Price { get; set; }
    }

    [FirestoreData]
    public class Promotions
    {
        [FirestoreProperty("activated")]
        [JsonProperty("activated")]
        public string Activated { get; set; }

        [FirestoreProperty("discount")]
        [JsonProperty("discount")]
        public string Discount { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
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
