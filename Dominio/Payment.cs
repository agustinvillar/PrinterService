using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Payment
    {
        [FirestoreProperty("paymentType")]
        public string PaymentType { get; set; }
        [FirestoreProperty("payMethod")]
        public string PaymentMethod { get; set; }
    }
}
