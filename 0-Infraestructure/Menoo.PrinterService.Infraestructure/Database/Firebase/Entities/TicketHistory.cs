using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class TicketHistory
    {
        [FirestoreProperty("printEvent")]
        [JsonProperty("printEvent")]
        public string PrintEvent { get; set; }

        [FirestoreProperty("dayCreatedAt")]
        [JsonProperty("dayCreatedAt")]
        public string DayCreatedAt { get; set; }

        [FirestoreProperty("createdAt")]
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty("printKey")]
        [JsonProperty("printKey")]
        public string PrintKey { get; set; }
    }
}
