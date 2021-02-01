using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.Entities
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
        [FirestoreProperty("categoryId")]
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        public CategoryStore CategoryStore => Store.CategoryStore.SingleOrDefault(s => s.Id == CategoryId);

        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("options")]
        [JsonProperty("options")]
        public ItemOption[] Options { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public double Price { get; set; }

        public double PriceToTicket => Total;

        [FirestoreProperty("quantity")]
        [JsonProperty("quantity")]
        public double Quantity { get; set; }

        public Store Store { get; set; }

        [FirestoreProperty("subTotal")]
        [JsonProperty("subTotal")]
        public double SubTotal { get; set; }
        
        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public double Total { get; set; }
        
        [FirestoreData]
        public class ItemOption
        {
            [FirestoreProperty("name")]
            [JsonProperty("name")]
            public string Name { get; set; }
            
            [FirestoreProperty("price")]
            [JsonProperty("price")]
            public double Price { get; set; }
        }

    }

    [FirestoreData]
    public class ItemOrderV2
    {
        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public string Total { get; set; }

        [FirestoreProperty("guestComment")]
        [JsonProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("promotions")]
        [JsonProperty("promotions")]
        public object Promotions { get; set; }

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

        public List<CategoryPromotions> MultiplePromotions { get; set; }

        public CategoryPromotions SinglePromotions { get; set; }
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

    public class SectorItem
    {
        public string ItemId { get; set; }

        [JsonProperty("sectors")]
        public List<PrintSettings> Sectors { get; set; }
    }
}
