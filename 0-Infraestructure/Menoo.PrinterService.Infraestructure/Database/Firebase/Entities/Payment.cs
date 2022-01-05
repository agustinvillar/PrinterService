using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Enums;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Payment
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("user")]
        [JsonProperty("user")]
        public User User { get; set; }

        [FirestoreProperty("taOpening")]
        [JsonProperty("taOpening")]
        public TakeAwayOpening TaOpening { get; set; }

        [FirestoreProperty("totalToPay")]
        [JsonProperty("totalToPay")]
        public double? TotalToPay { get; set; }

        [FirestoreProperty("payMethod")]
        [JsonProperty("payMethod")]
        public string PaymentMethod { get; set; }

        [FirestoreProperty("confirmationCode")]
        [JsonProperty("confirmationCode")]
        public string ConfirmationCode { get; set; }

        [FirestoreProperty("status")]
        [JsonProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("rut")]
        [JsonProperty("rut")]
        public string Rut { get; set; }

        [FirestoreProperty("paymentDate")]
        [JsonProperty("paymentDate")]
        public long? PaymentDate { get; set; }

        [FirestoreProperty("mailSended")]
        [JsonProperty("mailSended")]
        public bool? MailSended { get; set; }

        [FirestoreProperty("type")]
        [JsonProperty("type")]
        public string Type { get; set; }

        [FirestoreProperty("bookingId")]
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        [FirestoreProperty("expirationMonth")]
        [JsonProperty("expirationMonth")]
        public string ExpirationMonth { get; set; }

        [FirestoreProperty("expirationYear")]
        [JsonProperty("expirationYear")]
        public string ExpirationYear { get; set; }

        [FirestoreProperty("firstSixDigits")]
        [JsonProperty("firstSixDigits")]
        public string FirstSixDigits { get; set; }

        [FirestoreProperty("lastFourDigits")]
        [JsonProperty("lastFourDigits")]
        public string LastFourDigits { get; set; }

        [FirestoreProperty("paymentType")]
        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }

        [FirestoreProperty("paymentId")]
        [JsonProperty("paymentId")]
        public string PaymentId { get; set; }

        [FirestoreProperty("provider")]
        [JsonProperty("provider")]
        public int? Provider { get; set; }

        [FirestoreProperty("restoPercent")]
        [JsonProperty("restoPercent")]
        public double? RestoPercent { get; set; }

        [FirestoreProperty("menooPercent")]
        [JsonProperty("menooPercent")]
        public double? MenooPercent { get; set; }

        [FirestoreProperty("restoAmount")]
        [JsonProperty("restoAmount")]
        public double? RestoAmount { get; set; }

        [FirestoreProperty("menooAmount")]
        [JsonProperty("menooAmount")]
        public double? MenooAmount { get; set; }

        [FirestoreProperty("surcharge")]
        [JsonProperty("surcharge")]
        public double? Surcharge { get; set; }

        [FirestoreProperty("discounts")]
        [JsonProperty("discounts")]
        public List<Discount> Discounts { get; set; }
    }
}
