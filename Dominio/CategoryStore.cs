using Google.Cloud.Firestore;

namespace Dominio
{
    public class CategoryStore
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
