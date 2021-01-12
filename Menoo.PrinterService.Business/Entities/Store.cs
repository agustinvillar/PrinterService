using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Business.Entities
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

        public List<PrintSettings> GetPrintSettings(string printEvent) 
        {
            List<PrintSettings> printSettings = new List<PrintSettings>();
            var queryResult = this.Sectors?.FindAll(f => f.PrintEvents.Contains(printEvent) && f.AllowPrinting);
            if (queryResult != null && queryResult.Count > 0) 
            {
                printSettings.AddRange(queryResult);
            }
            return printSettings;
        }
    }
}
