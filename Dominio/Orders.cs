using Google.Cloud.Firestore;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class Orders
    {
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
    }
}
