using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class TableOpening
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("orders")]
        [JsonProperty("orders")]
        public Orders[] Orders { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("surcharge")]
        [JsonProperty("surcharge")]
        public int? Surcharge { get; set; }

        [FirestoreProperty("totalToPay")]
        [JsonProperty("totalToPay")]
        public double? TotalToPay { get; set; }

        [FirestoreProperty("propina")]
        [JsonProperty("propina")]
        public double? Tip { get; set; }

        [FirestoreProperty("discounts")]
        [JsonProperty("discounts")]
        public Discount[] Discounts { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        [JsonProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("paidByOther")]
        [JsonProperty("paidByOther")]
        public bool PaidByOther { get; set; }

        [FirestoreProperty("payWithPOS")]
        [JsonProperty("payWithPOS")]
        public bool PayWithPOS { get; set; }

        [FirestoreProperty("payingForAll")]
        [JsonProperty("payingForAll")]
        public bool PayingForAll { get; set; }

        [FirestoreProperty("artisticCutleryNumber")]
        [JsonProperty("artisticCutleryNumber")]
        public double? ArtisticCutleryQuantity { get; set; }

        [FirestoreProperty("artisticCutleryTotal")]
        [JsonProperty("artisticCutleryTotal")]
        public double? ArtisticCutleryTotal { get; set; }

        [FirestoreProperty("culteryPriceAmount")]
        [JsonProperty("culteryPriceAmount")]
        public double? CulteryPriceQuantity { get; set; }

        [FirestoreProperty("cutleryPriceTotal")]
        [JsonProperty("cutleryPriceTotal")]
        public double? CutleryPriceTotal { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        [JsonProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("payMethod")]
        [JsonProperty("payMethod")]
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
            [JsonProperty("Amount")]
            public double Amount { get; set; }

            [FirestoreProperty("Name")]
            [JsonProperty("Name")]
            public string Name { get; set; }

            [FirestoreProperty("Type")]
            [JsonProperty("Type")]
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

    public class TableOpeningV2 : TableOpening
    {
        public string TableNumberToShow { get; set; }

        public UserV2 User { get; set; }

        public string UserId { get; set; }

        public string CloseAt { get; set; }

        public string StoreId { get; set; }
    }
}
