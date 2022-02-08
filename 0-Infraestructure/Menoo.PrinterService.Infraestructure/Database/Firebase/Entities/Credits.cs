using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Credit
    {
        [FirestoreProperty("totalCredit")]
        [JsonProperty("detail")]
        public double TotalCredit { get; set; }

        [FirestoreProperty("totalCredit")]
        [JsonProperty("detail")]
        public CreditDetail Detail { get; set; }
    }

    [FirestoreData]
    public class CreditDetail 
    {
        [FirestoreProperty("paymentId")]
        [JsonProperty("paymentId")]
        public string PaymentId { get; set; }

        [FirestoreProperty("storeName")]
        [JsonProperty("storeName")]
        public string StoreName { get; set; }

        [FirestoreProperty("date")]
        [JsonProperty("date")]
        public string Date { get; set; }

        [FirestoreProperty("type")]
        [JsonProperty("type")]
        public object Type { get; set; }

        [FirestoreProperty("total")]
        [JsonProperty("total")]
        public double Total { get; set; }
    }
}
