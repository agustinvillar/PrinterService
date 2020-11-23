using Google.Cloud.Firestore;
using System.Collections.Generic;
using static Dominio.Item;

namespace Dominio.Entities
{
    [FirestoreData]
    public class OrderV2
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
        public int OrderNumber { get; set; }

        [FirestoreProperty("orderType")]
        public string OrderType { get; set; }

        [FirestoreProperty("userId")]
        public string UserId { get; set; }

        [FirestoreProperty("orderDate")]
        public string OrderDate { get; set; }

        [FirestoreProperty("updatedAt")]
        public int UpdatedAt { get; set; }

        [FirestoreProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }

        [FirestoreProperty("cancelSource")]
        public string CancelSource { get; set; }

        [FirestoreProperty("bookingId")]
        public int BookingId { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; }

        [FirestoreProperty("total")]
        public int Total { get; set; }

        [FirestoreProperty("printed")]
        public bool Printed { get; set; }
    }

    public class ItemV2
    {
        public string StoreId { get; set; }
        public int Total { get; set; }
        public string GuestComment { get; set; }
        public Promotions Promotions { get; set; }
        public int PriceWithDiscountTA { get; set; }
        public string PriceWithDiscount { get; set; }
        public int Quantity { get; set; }
        public int PriceTA { get; set; }
        public int Price { get; set; }
        public string CategoryId { get; set; }
        public ItemOption[] Options { get; set; }
        public int SubTotal { get; set; }
        public bool ItemIsTA { get; set; }
        public string Id { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }
        public string SoftId { get; set; }
    }

    public class StoreV2
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoImage { get; set; }
    }
}
