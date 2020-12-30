using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Booking
    {
        public enum PRINT_TYPE 
        {
            ACCEPTED, 
            CANCELLED 
        }

        [FirestoreProperty("bookingDate")]
        [JsonProperty("bookingDate")]
        public string BookingDate { get; set; }

        [FirestoreProperty("bookingNumber")]
        [JsonProperty("bookingNumber")]
        public long BookingNumber { get; set; }

        [FirestoreProperty("bookingState")]
        [JsonProperty("bookingState")]
        public string BookingState { get; set; }

        [FirestoreProperty("guestQuantity")]
        [JsonProperty("guestQuantity")]
        public int GuestQuantity { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("printedAccepted")]
        [JsonProperty("printedAccepted")]
        public bool PrintedAccepted { get; set; }

        [FirestoreProperty("printedCancelled")]
        [JsonProperty("printedCancelled")]
        public bool PrintedCancelled { get; set; }

        [FirestoreProperty("store")]
        [JsonProperty("store")]
        public Store Store { get; set; }

        [FirestoreProperty("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
