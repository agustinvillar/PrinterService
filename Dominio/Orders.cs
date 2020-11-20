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

    public class OrderV2
    {
        public string CancelSource { get; set; }

        public bool Closed { get; set; }

        public int Total { get; set; }

        public string Address { get; set; }

        public string UserId { get; set; }

        public string OrderType { get; set; }

        public StoreV2 Store { get; set; }

        public string TableOpeningFamilyId { get; set; }

        public int UpdatedAt { get; set; }

        public int BookingId { get; set; }

        public int OrderNumber { get; set; }

        public string TakeAwayHour { get; set; }

        public string GuestComment { get; set; }

        public string TableOpeningId { get; set; }

        public string UserName { get; set; }

        public string MadeAt { get; set; }

        public string OrderDate { get; set; }

        public List<ItemV2> Items { get; set; }

        public string Id { get; set; }

        public string Status { get; set; }

        public bool Printed { get; set; }
    }

    #region order v2
    public class StoreV2
    {
        public string LogoImage { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }

    public class Promotions
    {
        public string Name { get; set; }

        public string Discount { get; set; }

        public bool Activated { get; set; }
    }

    public class ItemV2
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public int Total { get; set; }

        public string PriceWithDiscount { get; set; }

        public string GuestComment { get; set; }

        public int PriceWithDiscountTA { get; set; }

        public int PriceTA { get; set; }

        public string CategoryId { get; set; }

        public string Id { get; set; }

        public int SubTotal { get; set; }

        public string Thumbnail { get; set; }

        public string StoreId { get; set; }

        public string SoftId { get; set; }

        public Promotions Promotions { get; set; }

        public object Options { get; set; }

        public bool ItemIsTA { get; set; }
    }
    #endregion
}
