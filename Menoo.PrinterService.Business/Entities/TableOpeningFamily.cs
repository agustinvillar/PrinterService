using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    public enum PrintedEvent
    {
        OPENING,
        CLOSING
    }

    public enum DiscountType
    {
        Surcharge = 0,
        Discount = 1,
        Normal = 2,
        Iva = 3
    }

    [FirestoreData]
    public class TableOpeningFamily
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("openedAt")]
        [JsonProperty("openedAt")]
        public string OpenedAt { get; set; }

        [FirestoreProperty("closedAt")]
        [JsonProperty("closedAt")]
        public string ClosedAt { get; set; }

        [FirestoreProperty("tableNumberId")]
        [JsonProperty("tableNumberId")]
        public int TableNumberId { get; set; }

        [FirestoreProperty("tableNumberToShow")]
        [JsonProperty("tableNumberToShow")]
        public int NumberToShow { get; set; }

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }

        public double TotalToPay { get; set; }

        [FirestoreProperty("tableOpenings")]
        [JsonProperty("tableOpenings")]
        public TableOpening[] TableOpenings { get; set; }

        [FirestoreProperty("pending")]
        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("openPrinted")]
        [JsonProperty("openPrinted")]
        public bool OpenPrinted { get; set; }

        [FirestoreProperty("closedPrinted")]
        [JsonProperty("closedPrinted")]
        public bool ClosedPrinted { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        [JsonProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        [JsonProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("propina")]
        [JsonProperty("propina")]
        public double? Tip { get; set; }

        [FirestoreProperty("requestPaymentCount")]
        [JsonProperty("requestPaymentCount")]
        public int RequestPaymentCount { get; set; }

        public string TableNumberToShow => NumberToShow > 0 ? NumberToShow.ToString() : TableNumberId.ToString();

        public double TotalToTicket(Store store)
        {
            switch (store.PaymentProvider)
            {
                case ProviderEnum.Geopay:
                    return TotalToPayWithSurcharge ?? 0;
                case ProviderEnum.MercadoPago:
                    return TotalPaidByClient ?? 0;
                case ProviderEnum.None:
                    return TotalToPayWithSurcharge ?? 0;
                default:
                    return 0.0;
            }
        }
    }

    [FirestoreData]
    public class TableOpening
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("orders")]
        [JsonProperty("orders")]
        public Order[] Orders { get; set; }

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
                case ProviderEnum.Geopay:
                    return TotalToPayWithSurcharge ?? 0;
                case ProviderEnum.MercadoPago:
                    return TotalPaidByClient ?? 0;
                case ProviderEnum.None:
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
