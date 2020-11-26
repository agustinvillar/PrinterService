using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    class Booking
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }
        [FirestoreProperty("bookingDate")]
        public string BookingDate { get; set; }
        [FirestoreProperty("bookingState")]
        public string BookingState { get; set; }
        [FirestoreProperty("bookingNumber")]
        public long BookingNumber { get; set; }
        [FirestoreProperty("guestQuantity")]
        public int GuestQuantity { get; set; }
        [FirestoreProperty("userId")]
        public string UserId { get; set; }
        [FirestoreProperty("printedAccepted")]
        public bool PrintedAccepted { get; set; }
        [FirestoreProperty("printedCancelled")]
        public bool PrintedCancelled { get; set; }
        [FirestoreProperty("store")]
        public Store Store { get; set; }
        public enum PRINT_TYPE { ACCEPTED, CANCELLED }
    }
}
