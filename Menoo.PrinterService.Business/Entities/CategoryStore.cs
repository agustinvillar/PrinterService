using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class CategoryStore
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
