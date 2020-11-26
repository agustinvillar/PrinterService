using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
