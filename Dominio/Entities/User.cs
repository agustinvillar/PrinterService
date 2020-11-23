using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
