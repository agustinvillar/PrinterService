using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSPrinterService
{
    [FirestoreData]
    class Orders
    {
        [FirestoreProperty("address")]
        public string Address {get; set;}        
        [FirestoreProperty ("printed")]
        public bool Printed { get; set; }
        [FirestoreProperty("bookingId")]
        public int BookingId { get; set; }
        [FirestoreProperty("items")]
        public  Item[] Items { get; set; }
        [FirestoreProperty("madeAt")]
        public string MadeAt { get; set; }
        [FirestoreProperty("orderDate")]
        public string OrderDate { get; set; }
        [FirestoreProperty("orderNumber")]
        public double OrderNumber { get; set; }
        [FirestoreProperty("orderType")]
        public string OrderType { get; set; }
        [FirestoreProperty("status")]
        public string Status { get; set; }
        [FirestoreProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }
        [FirestoreProperty("takeAwayHour")]
        public string TakeAwayHour { get; set; }
        [FirestoreProperty("total")]
        public double Total { get; set; }
        [FirestoreProperty("updatedAt")]
        public double UpdatedAt { get; set; }
        [FirestoreProperty("userId")]
        public string UserId { get; set; }
        [FirestoreProperty("userName")]
        public string UserName { get; set; }
    }
}
