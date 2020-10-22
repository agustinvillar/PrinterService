using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Discount
    {
        [FirestoreProperty("Amount")]
        public double Amount { get; set; }

        [FirestoreProperty("Name")]
        public string Name { get; set; }
        [FirestoreProperty("Type")]
        public DiscountType? Type { get; set; }

        public enum DiscountType
        {
            Surcharge = 0,
            Discount = 1,
            Normal = 2,
            Iva = 3
        }
    }
}
