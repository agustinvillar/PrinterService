using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Entities
{
    [FirestoreData]
    public class Payment
    {
        [FirestoreProperty("discounts")]
        [JsonProperty("discounts")]
        public Discount[] Discounts { get; set; }

        public List<string> DiscountsToTicketTa => Discounts != null
            ? Discounts.Where(d => d.DiscountType == Discount.DiscountTypeEnum.Discount)
                .Select(d => $"{d.Name} -${d.Amount}").ToList()
            : new List<string>();

        [FirestoreProperty("payMethod")]
        [JsonProperty("payMethod")]
        public string PaymentMethod { get; set; }

        [FirestoreProperty("paymentType")]
        [JsonProperty("paymentType")]
        public string PaymentType { get; set; }
        
        [FirestoreProperty("surcharge")]
        [JsonProperty("surcharge")]
        public double? Surcharge { get; set; }

        [FirestoreProperty("totalToPay")]
        [JsonProperty("totalToPay")]
        public double TotalToPay { get; set; }
        
        public double TotalToPayTicket => TotalToPay +
            Discounts?.Where(d => d.DiscountType == Discount.DiscountTypeEnum.Iva)
                .Sum(d => d.Amount) ?? TotalToPay;

        [FirestoreData]
        public class Discount
        {
            public enum DiscountTypeEnum
            {
                Surcharge = 0,
                Discount = 1,
                Normal = 2,
                Iva = 3
            }

            [FirestoreProperty("amount")]
            [JsonProperty("amount")]
            public double Amount { get; set; }

            public DiscountTypeEnum DiscountType => (DiscountTypeEnum)Type;

            [FirestoreProperty("name")]
            [JsonProperty("name")]
            public string Name { get; set; }
            
            [FirestoreProperty("type")]
            [JsonProperty("type")]
            public int Type { get; set; }
        }
    }
}
