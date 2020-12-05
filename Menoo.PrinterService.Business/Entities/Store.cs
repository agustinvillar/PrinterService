using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using System.Collections.Generic;
using System.Linq;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Store
    {
        [FirestoreProperty("id")]
        public string StoreId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("sectors")]
        public List<PrintSettings> Sectors { get; set; }

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
