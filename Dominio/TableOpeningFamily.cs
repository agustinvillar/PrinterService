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

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }
        public double TotalToPay { get; set; }
        public double TotalToPayWithPropina { get; set; }
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

        public enum PRINTED_EVENT
        {
            OPENING,
            CLOSING
        }

    }
}
