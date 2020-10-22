using System.Linq;
using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Payment
    {
        [FirestoreProperty("paymentType")]
        public string PaymentType { get; set; }

        [FirestoreProperty("payMethod")]
        public string PaymentMethod { get; set; }

        [FirestoreProperty("discounts")]
        public Discount[] Discounts { get; set; }

        [FirestoreProperty("totalToPay")]
        public double TotalToPay { get; set; }

        public double TotalToPayTicket => TotalToPay - Discounts?.Where(d => d.Type == Discount.DiscountType.Iva).Sum(d => d.Amount) ?? TotalToPay;
    }
}
