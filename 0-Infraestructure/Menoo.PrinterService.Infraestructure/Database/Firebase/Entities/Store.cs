using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
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

    public class UnifiedSectorData
    {
        [JsonProperty("unifiedTicketSectorId")]
        [FirestoreProperty("unifiedTicketSectorId")]
        public string UnifiedTicketSectorId { get; set; }
    }

    [FirestoreData]
    public class Store
    {
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

        [FirestoreProperty("printSettings")]
        [JsonProperty("printSettings")]
        public UnifiedSectorData UnifiedTicket { get; set; }
    }
}
