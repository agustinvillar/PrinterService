using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class PrintSettings
    {
        [FirestoreProperty("allowPrinting")]
        [JsonProperty("allowPrinting")]
        public bool AllowPrinting { get; set; }

        [FirestoreProperty("copies")]
        [JsonProperty("copies")]
        public int Copies { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("printer")]
        [JsonProperty("printer")]
        public string Printer { get; set; }

        [FirestoreProperty("printEvents")]
        [JsonProperty("printEvents")]
        public List<string> PrintEvents { get; set; }

        [FirestoreProperty("printQR")]
        [JsonProperty("printQR")]
        public bool PrintQR { get; set; }

        [FirestoreProperty("updatedAt")]
        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [FirestoreProperty("sectorName")]
        [JsonProperty("sectorName")]
        public string SectorName { get; set; }

        [FirestoreProperty("isHTML")]
        [JsonProperty("isHTML")]
        public bool IsHTML { get; set; }
    }
}
