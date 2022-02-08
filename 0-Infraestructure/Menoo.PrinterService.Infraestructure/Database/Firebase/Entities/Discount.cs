using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Enums;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Discount
    {
        [FirestoreProperty("amount")]
        [JsonProperty("amount")]
        public double? Amount { get; set; }

        public DiscountTypeEnum DiscountType => (DiscountTypeEnum)Type;

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("type")]
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
