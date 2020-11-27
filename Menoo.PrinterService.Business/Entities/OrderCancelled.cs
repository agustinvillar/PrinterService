using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class OrderCancelled
    {
        [FirestoreProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("store")]
        public StoreV2 Store { get; set; }

        [FirestoreProperty("userName")]
        public string UserName { get; set; }

        [FirestoreProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }

        [FirestoreProperty("address")]
        public string Address { get; set; }

        [FirestoreProperty("madeAt")]
        public string MadeAt { get; set; }

        [FirestoreProperty("cancelMotive")]
        public string CancelMotive { get; set; }

        [FirestoreProperty("items")]
        public List<ItemV2> Items { get; set; }

        [FirestoreProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }

        [FirestoreProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [FirestoreProperty("orderType")]
        public string OrderType { get; set; }

        [FirestoreProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("orderDate")]
        public string OrderDate { get; set; }

        [FirestoreProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [FirestoreProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }

        [FirestoreProperty("cancelSource")]
        public string CancelSource { get; set; }

        [FirestoreProperty("bookingId")]
        public string BookingId { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("total")]
        public string Total { get; set; }
    }

    [FirestoreData]
    public class ItemV2
    {
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("total")]
        public string Total { get; set; }

        [FirestoreProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("promotions")]
        public Promotions Promotions { get; set; }

        [FirestoreProperty("priceWithDiscountTA")]
        public string PriceWithDiscountTA { get; set; }

        [FirestoreProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; }

        [FirestoreProperty("quantity")]
        public string Quantity { get; set; }

        [FirestoreProperty("priceTA")]
        public string PriceTA { get; set; }

        [FirestoreProperty("price")]
        public string Price { get; set; }

        [FirestoreProperty("categoryId")]
        public string CategoryId { get; set; }

        [FirestoreProperty("options")]
        public ItemOptionV2[] Options { get; set; }

        [FirestoreProperty("subTotal")]
        public string SubTotal { get; set; }

        [FirestoreProperty("itemIsTA")]
        public string ItemIsTA { get; set; }

        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("softId")]
        public string SoftId { get; set; }
    }

    [FirestoreData]
    public class StoreV2
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("logoImage")]
        public string LogoImage { get; set; }
    }

    [FirestoreData]
    public class ItemOptionV2
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("price")]
        public string Price { get; set; }
    }

    [FirestoreData]
    public class Promotions
    {
        [FirestoreProperty("activated")]
        public string Activated { get; set; }

        [FirestoreProperty("discount")]
        public string Discount { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }
    }

    public static class OrderPrintedExtensions 
    {
        public static bool IsPrinted(this DocumentSnapshot snapshot) 
        {
            try
            {
                var document = snapshot.ToDictionary();
                return document.ContainsKey("printed");
            }
            catch 
            {
                return false;
            }
        }

        public static OrderCancelled GetOrderData(this DocumentSnapshot snapshot) 
        {
            var document = snapshot.ToDictionary();
            var objectCasted = Utils.GetObject<OrderCancelled>(document);
            return objectCasted;
        }
    }
}
