using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Queues
{
    public sealed class PrintMessage
    {
        public string PrintEvent { get; set; }

        public string DocumentId { get; set; }

        public List<string> DocumentsId { get; set; }

        public string TypeDocument { get; set; }

        public string SubTypeDocument { get; set; }

        public string Builder { get; set; }

        public string ExtraData { get; set; }
    }

    [FirestoreData]
    public sealed class DocumentMessage 
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
    }
}
