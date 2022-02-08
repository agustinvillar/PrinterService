using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreProperty("copies")]
        [JsonProperty("copies")]
        public int Copies { get; set; }

        [FirestoreProperty("date")]
        [JsonProperty("date")]
        public string Date { get; set; }

        [FirestoreProperty("expired")]
        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [FirestoreProperty("printBefore")]
        [JsonProperty("printBefore")]
        public string PrintBefore { get; set; }

        [FirestoreProperty("printData")]
        [JsonProperty("printData")]
        public string TicketImage { get; set; }

        [FirestoreProperty("printed")]
        [JsonProperty("printed")]
        public bool Printed { get; set; }

        [FirestoreProperty("printedAt")]
        [JsonProperty("printedAt")]
        public string PrintedAt { get; set; }

        [FirestoreProperty("printer")]
        [JsonProperty("printer")]
        public string PrinterName { get; set; }

        [FirestoreProperty("storeName")]
        [JsonProperty("storeName")]
        public string StoreName { get; set; }

        [FirestoreProperty("ticketType")]
        [JsonProperty("ticketType")]
        public string TicketType { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }
    }
}
