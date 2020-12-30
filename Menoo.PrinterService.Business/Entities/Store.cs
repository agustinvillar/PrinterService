using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Store
    {
        public enum ProviderEnum
        {
            None = 0,
            MercadoPago = 1,
            Geopay = 2
        }

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
        public string StoreId { get; set; }

        public bool AllowPrint(string printEvent = "")
        {
            if (!string.IsNullOrEmpty(printEvent) && Sectors != null)
            {
                return this.Sectors != null && this.Sectors.Any(f => f.PrintEvents.Contains(printEvent) && f.AllowPrinting);
            }
            else
            {
                return AllowPrinting.GetValueOrDefault();
            }
        }
    }
}
