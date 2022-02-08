using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class CategoryPromotions
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

    [FirestoreData]
    public class ItemOrder
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public double? Price { get; set; }

        [FirestoreProperty("categoryId")]
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("quantity")]
        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [FirestoreProperty("thumbnail")]
        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [FirestoreProperty("itemIsTA")]
        [JsonProperty("itemIsTA")]
        public bool? ItemIsTA { get; set; }

        [FirestoreProperty("options")]
        [JsonProperty("options")]
        public List<ItemOption> Options { get; set; }

        [FirestoreProperty("priceTA")]
        [JsonProperty("priceTA")]
        public double? PriceTA { get; set; }

        [FirestoreProperty("priceWithDiscount")]
        [JsonProperty("priceWithDiscount")]
        public double? PriceWithDiscount { get; set; }

        [FirestoreProperty("priceWithDiscountTA")]
        [JsonProperty("priceWithDiscountTA")]
        public double? PriceWithDiscountTA { get; set; }

        [FirestoreProperty("priceWithDiscountMarket")]
        [JsonProperty("priceWithDiscountMarket")]
        public double? PriceWithDiscountMarket { get; set; }

        [FirestoreProperty("promotions")]
        [JsonProperty("promotions")]
        public object Promotions { get; set; }

        [FirestoreProperty("softId")]
        [JsonProperty("softId")]
        public string SoftId { get; set; }

        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        public List<CategoryPromotions> MultiplePromotions { get; set; }

        public CategoryPromotions SinglePromotions { get; set; }

        [FirestoreProperty("sectors")]
        [JsonProperty("sectors")]
        public List<PrintSettings> Sectors { get; set; }

        [FirestoreProperty("subTotal")]
        [JsonProperty("subTotal")]
        public double? Subtotal { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public double? Total { get; set; }
    }

    [FirestoreData]
    public class ItemOption
    {
        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public string Price { get; set; }
    }

    public class SectorItem
    {
        public string ItemId { get; set; }

        [JsonProperty("sectors")]
        public List<PrintSettings> Sectors { get; set; }
    }
}
