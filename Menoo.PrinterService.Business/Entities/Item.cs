using System.Linq;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Item
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
}
