using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Entities
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
}
