using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    class User
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
