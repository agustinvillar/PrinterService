using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class TableOpeningFamily
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("openedAt")]
        [JsonProperty("openedAt")]
        public string OpenedAt { get; set; }

        [FirestoreProperty("closedAt")]
        [JsonProperty("closedAt")]
        public string ClosedAt { get; set; }

        [FirestoreProperty("tableNumberId")]
        [JsonProperty("tableNumberId")]
        public int TableNumberId { get; set; }

        [FirestoreProperty("tableNumberToShow")]
        [JsonProperty("tableNumberToShow")]
        public int NumberToShow { get; set; }

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }

        public double TotalToPay { get; set; }

        [FirestoreProperty("tableOpenings")]
        [JsonProperty("tableOpenings")]
        public TableOpening[] TableOpenings { get; set; }

        [FirestoreProperty("pending")]
        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("pending")]
        public string StoreId { get; set; }

        [FirestoreProperty("openPrinted")]
        [JsonProperty("pending")]
        public bool OpenPrinted { get; set; }

        [FirestoreProperty("closedPrinted")]
        [JsonProperty("closedPrinted")]
        public bool ClosedPrinted { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        [JsonProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        [JsonProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("propina")]
        [JsonProperty("propina")]
        public double? Tip { get; set; }

        [FirestoreProperty("requestPaymentCount")]
        [JsonProperty("requestPaymentCount")]
        public int RequestPaymentCount { get; set; }

        public string TableNumberToShow => NumberToShow != null ? NumberToShow.ToString() : TableNumberId.ToString();

        public double TotalToTicket(Store store)
        {
            switch (store.PaymentProvider)
            {
                case Store.ProviderEnum.Geopay:
                    return TotalToPayWithSurcharge ?? 0;
                case Store.ProviderEnum.MercadoPago:
                    return TotalPaidByClient ?? 0;
                case Store.ProviderEnum.None:
                    return TotalToPayWithSurcharge ?? 0;
                default:
                    return 0.0;
            }
        }

        public enum PrintedEvent
        {
            OPENING,
            CLOSING
        }
    }
}
