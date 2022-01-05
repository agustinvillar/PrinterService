using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{ 
    [FirestoreData]
    public class User
    {
        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("email")]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
