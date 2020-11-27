using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class TableOpening
    {
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

        [FirestoreProperty("paidByOther")]
        public bool PaidByOther { get; set; }

        [FirestoreProperty("payingForAll")]
        public bool PayingForAll { get; set; }

        [FirestoreProperty("artisticCutleryNumber")]
        public double? ArtisticCutleryQuantity { get; set; }

        [FirestoreProperty("artisticCutleryTotal")]
        public double? ArtisticCutleryTotal { get; set; }

        [FirestoreProperty("culteryPriceAmount")]
        public double? CulteryPriceQuantity { get; set; }

        [FirestoreProperty("cutleryPriceTotal")]
        public double? CutleryPriceTotal { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("payMethod")]
        public string PayMethod { get; set; }

        public bool PagoPorTodos => PayingForAll;
        public bool AlguienLePago => PaidByOther;
        public bool PagoPorElMismo => !PaidByOther && !PayingForAll;

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

        [FirestoreData]
        public class Discount
        {
            [FirestoreProperty("Amount")]
            public double Amount { get; set; }

            [FirestoreProperty("Name")]
            public string Name { get; set; }
            [FirestoreProperty("Type")]
            public DiscountType? Type { get; set; }

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
