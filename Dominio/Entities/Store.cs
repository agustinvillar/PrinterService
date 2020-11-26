using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business
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
        public string PaymentProviderString { get; set; }

        public ProviderEnum PaymentProvider => string.IsNullOrEmpty(PaymentProviderString)
            ? ProviderEnum.None
            : (ProviderEnum)int.Parse(PaymentProviderString);

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
