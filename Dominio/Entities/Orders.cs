using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Dominio
{
    [FirestoreData]
    public class Orders
    {
        [FirestoreProperty("address")]
        public virtual string Address { get; set; }
        [FirestoreProperty("printed")]
        public virtual bool Printed { get; set; }
        [FirestoreProperty("items")]
        public virtual Item[] Items { get; set; }
        [FirestoreProperty("madeAt")]
        public virtual string MadeAt { get; set; }
        [FirestoreProperty("orderDate")]
        public virtual string OrderDate { get; set; }
        [FirestoreProperty("orderType")]
        public virtual string OrderType { get; set; }
        [FirestoreProperty("tableOpeningFamilyId")]
        public virtual string TableOpeningFamilyId { get; set; }
        [FirestoreProperty("takeAwayHour")]
        public virtual string TakeAwayHour { get; set; }
        [FirestoreProperty("userName")]
        public virtual string UserName { get; set; }
        [FirestoreProperty("storeId")]
        public virtual string StoreId { get; set; }
        public virtual string Id { get; set; }

        private Store _store;
        public virtual Store Store
        {
            get => _store;
            set
            {
                _store = value;
                foreach (var item in Items) item.Store = value;
            }
        }

        public virtual bool IsTakeAway => OrderType.ToUpper() == "TAKEAWAY";
    }

    public class StoreV2
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoImage { get; set; }
    }

    public class Promotions
    {
        public bool Activated { get; set; }
        public string Discount { get; set; }
        public string Name { get; set; }
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
        public object Options { get; set; }
        public int SubTotal { get; set; }
        public bool ItemIsTA { get; set; }
        public string Id { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }
        public string SoftId { get; set; }
    }

    [FirestoreData]
    public class OrderV2
    {
        [FirestoreProperty("guestComment")]
        public string GuestComment { get; set; }

        [FirestoreProperty("store")]
        public Store Store { get; set; }

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
        public List<Item> Items { get; set; }

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
}
