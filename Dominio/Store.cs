using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Store
    {
        [FirestoreProperty("id")]
        public string StoreId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("allowPrinting")]
        public bool? AllowPrinting { get; set; }

        [FirestoreProperty("paymentProvider")]
        public ProviderEnum PaymentProvider { get; set; }

        [FirestoreProperty("categoryStore")]
        public CategoryStore[] CategoryStore { get; set; }

        public enum ProviderEnum
        {
            None = 0,
            MercadoPago = 1,
            Geopay = 2
        }
    }
}
