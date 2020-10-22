using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class TableOpening
    {
        public int CulteryPrice { get; set; }

        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("orders")]
        public Orders[] Orders { get; set; }

        [FirestoreProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("surcharge")]
        public int? Surcharge { get; set; }

        [FirestoreProperty("totalToPay")]
        public double? TotalToPay { get; set; }

        [FirestoreProperty("propina")]
        public double? Tip { get; set; }

        [FirestoreProperty("discounts")]
        public Discount[] Discounts { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreData]
        public class Discount
        {
            [FirestoreProperty("Amount")]
            public double Amount { get; set; }

            [FirestoreProperty("Name")]
            public string Name { get; set; }
            [FirestoreProperty("Type")]
            public DiscountType Type { get; set; }

            public enum DiscountType
            {
                Surcharge = 0,
                Discount = 1,
                Normal = 2,
                Iva = 3
            }
        }
    }
}
