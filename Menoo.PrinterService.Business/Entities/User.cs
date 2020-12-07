using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Entities 
{ 
    [FirestoreData]
    public class User
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }
    }

    public class UserV2 
    {
        public string Email { get; set; }

        public string Name { get; set; }
        
    }
}
