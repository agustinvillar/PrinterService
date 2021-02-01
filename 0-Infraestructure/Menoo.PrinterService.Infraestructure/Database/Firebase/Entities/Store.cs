using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    public enum ProviderEnum
    {
        None = 0,
        MercadoPago = 1,
        Geopay = 2
    }

    [FirestoreData]
    public class CategoryStore
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    [FirestoreData]
    public class Store
    {
        [FirestoreProperty("allowPrinting")]
        [JsonProperty("allowPrinting")]
        public bool? AllowPrinting { get; set; }

        [FirestoreProperty("categoryStore")]
        [JsonProperty("categoryStore")]
        public CategoryStore[] CategoryStore { get; set; }

        [FirestoreProperty("name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        public ProviderEnum PaymentProvider => string.IsNullOrEmpty(PaymentProviderString)
            ? ProviderEnum.None
            : (ProviderEnum)int.Parse(PaymentProviderString);

        [FirestoreProperty("paymentProvider")]
        [JsonProperty("paymentProvider")]
        public string PaymentProviderString { get; set; }

        [FirestoreProperty("sectors")]
        [JsonProperty("sectors")]
        public List<PrintSettings> Sectors { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("logoImage")]
        [JsonProperty("logoImage")]
        public string LogoImage { get; set; }
    }
}
