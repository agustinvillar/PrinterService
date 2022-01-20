using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
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
        public int NumberId { get; set; }

        [FirestoreProperty("tableNumberToShow")]
        [JsonProperty("tableNumberToShow")]
        public int? NumberToShow { get; set; }

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }

        public double TotalToPay { get; set; }

        [FirestoreProperty("tableOpenings")]
        [JsonProperty("tableOpenings")]
        public List<TableOpening> TableOpenings { get; set; }

        [FirestoreProperty("pending")]
        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

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

        public string TableNumberToShow => NumberToShow == 0 || NumberToShow == null ? NumberId.ToString() : NumberToShow.ToString();

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
        [FirestoreProperty("observations")]
        [JsonProperty("observations")]
        public string Observations { get; set; }

        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("openendAt")]
        [JsonProperty("openendAt")]
        public string OpenendAt { get; set; }

        [FirestoreProperty("closedAt")]
        [JsonProperty("closedAt")]
        public string ClosedAt { get; set; }

        [FirestoreProperty("closed")]
        [JsonProperty("closed")]
        public bool? Closed { get; set; }

        [FirestoreProperty("tableFamilyId")]
        [JsonProperty("tableFamilyId")]
        public string TableFamilyId { get; set; }

        [FirestoreProperty("totalToPay")]
        [JsonProperty("totalToPay")]
        public double? TotalToPay { get; set; }

        [FirestoreProperty("totalToPayWithPropina")]
        [JsonProperty("totalToPayWithPropina")]
        public double? TotalToPayWithPropina { get; set; }

        [FirestoreProperty("offerCupon")]
        [JsonProperty("offerCupon")]
        public OfferCoupon OfferCoupon { get; set; }

        [FirestoreProperty("propina")]
        [JsonProperty("propina")]
        public double? Propina { get; set; }

        [FirestoreProperty("discountAmmount")]
        [JsonProperty("discountAmmount")]
        public double? DiscountAmmount { get; set; }

        [FirestoreProperty("discountByCouponAmount")]
        [JsonProperty("discountByCouponAmount")]
        public double? DiscountByCouponAmount { get; set; }

        [FirestoreProperty("discountByCreditAmount")]
        [JsonProperty("discountByCreditAmount")]
        public double? DiscountByCreditAmount { get; set; }

        [FirestoreProperty("payMethod")]
        [JsonProperty("payMethod")]
        public string PayMethod { get; set; }

        [FirestoreProperty("confirmationCash")]
        [JsonProperty("confirmationCash")]
        public bool? ConfirmationCash { get; set; }

        [FirestoreProperty("cashPaymentConfirmed")]
        [JsonProperty("cashPaymentConfirmed")]
        public bool? CashPaymentConfirmed { get; set; }

        [FirestoreProperty("orders")]
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }

        [FirestoreProperty("payWithPOS")]
        [JsonProperty("payWithPOS")]
        public bool PayWithPOS { get; set; }

        [FirestoreProperty("promotionalCode")]
        [JsonProperty("promotionalCode")]
        public OfferCoupon PromotionalCode { get; set; }

        [FirestoreProperty("tipAmount")]
        [JsonProperty("tipAmount")]
        public double? TipAmount { get; set; }

        [FirestoreProperty("tipPercentage")]
        [JsonProperty("tipPercentage")]
        public double? TipPercentage { get; set; }

        [FirestoreProperty("capacity")]
        [JsonProperty("capacity")]
        public int? Capacity { get; set; }

        [FirestoreProperty("paidByOther")]
        [JsonProperty("paidByOther")]
        public bool PaidByOther { get; set; }

        [FirestoreProperty("payingForAll")]
        [JsonProperty("payingForAll")]
        public bool PayingForAll { get; set; }

        [FirestoreProperty("culteryPriceAmount")]
        [JsonProperty("culteryPriceAmount")]
        public double? CulteryPriceQuantity { get; set; }

        [FirestoreProperty("cutleryPriceTotal")]
        [JsonProperty("cutleryPriceTotal")]
        public double? CutleryPriceTotal { get; set; }

        [FirestoreProperty("isPayingFlow")]
        [JsonProperty("isPayingFlow")]
        public bool? IsPayingFlow { get; set; }

        [FirestoreProperty("surcharge")]
        [JsonProperty("surcharge")]
        public int? Surcharge { get; set; }

        [FirestoreProperty("discounts")]
        [JsonProperty("discounts")]
        public List<Discount> Discounts { get; set; }

        [FirestoreProperty("totalPaidByClient")]
        [JsonProperty("totalPaidByClient")]
        public double? TotalPaidByClient { get; set; }

        [FirestoreProperty("artisticCutlery")]
        [JsonProperty("artisticCutlery")]
        public int? ArtisticCutlery { get; set; }

        [FirestoreProperty("artisticCutleryTotal")]
        [JsonProperty("artisticCutleryTotal")]
        public int? ArtisticCutleryTotal { get; set; }

        [FirestoreProperty("artisticCutleryNumber")]
        [JsonProperty("artisticCutleryNumber")]
        public double? ArtisticCutleryQuantity { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        [JsonProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("paymentId")]
        [JsonProperty("paymentId")]
        public int? PaymentId { get; set; }

        public bool PaidForIt => !PaidByOther && !PayingForAll;
    }

    [FirestoreData]
    public class TakeAwayOpening
    {
        [FirestoreProperty("id")]
        [JsonProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("userId")]
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("observations")]
        [JsonProperty("observations")]
        public string Observations { get; set; }

        [FirestoreProperty("takeAwayHour")]
        [JsonProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }

        [FirestoreProperty("orders")]
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }

        [FirestoreProperty("totalToPay")]
        [JsonProperty("totalToPay")]
        public double? TotalToPay { get; set; }

        [FirestoreProperty("offerCupon")]
        [JsonProperty("offerCupon")]
        public OfferCoupon OfferCoupon { get; set; }

        [FirestoreProperty("credit")]
        [JsonProperty("credit")]
        public Credit Credit { get; set; }

        [FirestoreProperty("payMethod")]
        [JsonProperty("payMethod")]
        public string PayMethod { get; set; }

        [FirestoreProperty("subTotal")]
        [JsonProperty("subTotal")]
        public double? SubTotal { get; set; }

        [FirestoreProperty("extras")]
        [JsonProperty("extras")]
        public List<Extra> Extras { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("userPhone")]
        [JsonProperty("userPhone")]
        public string UserPhone { get; set; }

        [FirestoreProperty("discountAmmount")]
        [JsonProperty("discountAmmount")]
        public double? DiscountAmmount { get; set; }

        [FirestoreProperty("discountByCouponAmount")]
        [JsonProperty("discountByCouponAmount")]
        public double? DiscountByCouponAmount { get; set; }

        [FirestoreProperty("discountByCreditAmount")]
        [JsonProperty("discountByCreditAmount")]
        public double? DiscountByCreditAmount { get; set; }

        [FirestoreProperty("paySurcharge")]
        [JsonProperty("paySurcharge")]
        public bool? PaySurcharge { get; set; }

        [FirestoreProperty("storeLogoImage")]
        [JsonProperty("storeLogoImage")]
        public string StoreLogoImage { get; set; }

        [FirestoreProperty("paymentId")]
        [JsonProperty("paymentId")]
        public int PaymentId { get; set; }
    }
}
