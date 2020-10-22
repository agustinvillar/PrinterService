using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreProperty("ticketType")]
        public string TicketType { get; set; }
        [FirestoreProperty("printData")]
        public string Data { get; set; }
        [FirestoreProperty("printed")]
        public bool Printed { get; set; }
        [FirestoreProperty("printedAt")]
        public string PrintedAt { get; set; }
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }
        [FirestoreProperty("printBefore")]
        public string PrintBefore { get; set; }
        [FirestoreProperty("expired")]
        public bool Expired { get; set; }
        [FirestoreProperty("date")]
        public string Date { get; set; }
        [FirestoreProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }

        public enum TicketTypeEnum
        {
            OPEN_TABLE,
            CLOSE_TABLE,
            ORDER,
            NEW_BOOKING,
            CANCELLED_BOOKING
        }
    }
}
