using Google.Cloud.Firestore;
using Newtonsoft.Json;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Orders
    {
        [FirestoreProperty("address")]
        [JsonProperty("address")]
        public virtual string Address { get; set; }

        [FirestoreProperty("printed")]
        [JsonProperty("printed")]
        public virtual bool Printed { get; set; }

        [FirestoreProperty("items")]
        [JsonProperty("items")]
        public virtual Item[] Items { get; set; }

        [FirestoreProperty("madeAt")]
        [JsonProperty("madeAt")]
        public virtual string MadeAt { get; set; }

        [FirestoreProperty("orderDate")]
        [JsonProperty("orderDate")]
        public virtual string OrderDate { get; set; }

        [FirestoreProperty("orderType")]
        [JsonProperty("orderType")]
        public virtual string OrderType { get; set; }

        [FirestoreProperty("tableOpeningFamilyId")]
        [JsonProperty("tableOpeningFamilyId")]
        public virtual string TableOpeningFamilyId { get; set; }

        [FirestoreProperty("takeAwayHour")]
        [JsonProperty("takeAwayHour")]
        public virtual string TakeAwayHour { get; set; }

        [FirestoreProperty("userName")]
        [JsonProperty("userName")]
        public virtual string UserName { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public virtual string StoreId { get; set; }

        [FirestoreProperty("orderNumber")]
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [FirestoreProperty("bookingId")]
        [JsonProperty("bookingId")]
        public string BookingId { get; set; }

        public virtual string Id { get; set; }

        private Store _store;

        public virtual Store Store
        {
            get => _store;
            set
            {
                _store = value;
                if (Items != null) 
                {
                    foreach (var item in Items)
                    {
                        item.Store = value;
                    }
                }
            }
        }

        public virtual bool IsTakeAway => OrderType.ToUpper() == "TAKEAWAY";
    }
}
