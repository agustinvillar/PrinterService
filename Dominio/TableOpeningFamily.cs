using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class TableOpeningFamily
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("openedAt")]
        public string OpenedAt { get; set; }

        [FirestoreProperty("closedAt")]
        public string ClosedAt { get; set; }

        [FirestoreProperty("tableNumberId")]
        public int TableNumberId { get; set; }
        [FirestoreProperty("tableNumberToShow")]
        public int? NumberToShow { get; set; }

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }
        public double TotalToPay { get; set; }
        [FirestoreProperty("tableOpenings")]
        public TableOpening[] TableOpenings { get; set; }

        [FirestoreProperty("pending")]
        public bool? Pending { get; set; }
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("openPrinted")]
        public bool OpenPrinted { get; set; }

        [FirestoreProperty("closedPrinted")]
        public bool ClosedPrinter { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("propina")]
        public double? Tip { get; set; }

        public string TableNumberToShow => NumberToShow.HasValue ? NumberToShow.ToString() : TableNumberId.ToString();
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
