using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public sealed class PrintEventMessage
    {
        [FirestoreProperty("entityId")]
        [JsonProperty("entityId")]
        public string EntityId { get; set; }

        [FirestoreProperty("entityIdArray")]
        [JsonProperty("entityIdArray")]
        public List<string> EntityIdArray { get; set; }

        [FirestoreProperty("event")]
        [JsonProperty("event")]
        public string Event { get; set; }

        [FirestoreProperty("read")]
        [JsonProperty("read")]
        public bool Read { get; set; }

        [FirestoreProperty("tableOpId")]
        [JsonProperty("tableOpId")]
        public string TableOpeningId { get; set; }
    }
}
