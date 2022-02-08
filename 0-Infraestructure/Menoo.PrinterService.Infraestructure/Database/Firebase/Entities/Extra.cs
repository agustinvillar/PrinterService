using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Extra
    {
        [FirestoreProperty("softId")]
        [JsonProperty("softId")]
        public string SoftId { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [FirestoreProperty("type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [FirestoreProperty("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [FirestoreProperty("checked")]
        [JsonProperty("checked")]
        public bool? Checked { get; set; }
    }
}
