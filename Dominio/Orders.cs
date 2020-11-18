using Google.Cloud.Firestore;

namespace Dominio
{
    [FirestoreData]
    public class Orders
    {
        [FirestoreProperty("address")]
        public string Address { get; set; }
        [FirestoreProperty("printed")]
        public bool Printed { get; set; }
        [FirestoreProperty("items")]
        public Item[] Items { get; set; }
        [FirestoreProperty("madeAt")]
        public string MadeAt { get; set; }
        [FirestoreProperty("orderDate")]
        public string OrderDate { get; set; }
        [FirestoreProperty("orderType")]
        public string OrderType { get; set; }
        [FirestoreProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }
        [FirestoreProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }
        [FirestoreProperty("userName")]
        public string UserName { get; set; }
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }
        public string Id { get; set; }

        private Store _store;
        public Store Store
        {
            get => _store;
            set
            {
                _store = value;
                foreach (var item in Items) item.Store = value;
            }
        }

        public bool IsTakeAway => OrderType.ToUpper() == "TAKEAWAY";
    }


}
