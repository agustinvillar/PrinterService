using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business
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

        [FirestoreProperty("surcharge")]
        public double? Surcharge { get; set; }


        public List<string> DiscountsToTicketTa => Discounts != null
            ? Discounts.Where(d => d.DiscountType == Discount.DiscountTypeEnum.Discount)
                .Select(d => $"{d.Name} -${d.Amount}").ToList()
            : new List<string>();

        public double TotalToPayTicket => TotalToPay +
            Discounts?.Where(d => d.DiscountType == Discount.DiscountTypeEnum.Iva)
                .Sum(d => d.Amount) ?? TotalToPay;

        [FirestoreData]
        public class Discount
        {
            [FirestoreProperty("amount")]
            public double Amount { get; set; }

            [FirestoreProperty("name")]
            public string Name { get; set; }
            [FirestoreProperty("type")]
            public int Type { get; set; }

            public DiscountTypeEnum DiscountType => (DiscountTypeEnum)Type;

            public enum DiscountTypeEnum
            {
                Surcharge = 0,
                Discount = 1,
                Normal = 2,
                Iva = 3
            }
        }
    }
}
