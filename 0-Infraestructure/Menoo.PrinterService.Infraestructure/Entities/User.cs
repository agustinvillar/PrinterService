using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Entities 
{ 
    [FirestoreData]
    public class User
    {
        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UserV2 
    {
        public string Email { get; set; }

        public string Name { get; set; }
        
    }
}
