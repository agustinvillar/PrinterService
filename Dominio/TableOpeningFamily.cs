using System;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class TableOpeningFamily
    {
        [FirestoreProperty("activeGuestQuantity")]
        public int ActiveGuestQuantity { get; set; }
        [FirestoreProperty("activityLog")]
        public ActivityLog[] ActivityLog { get; set; }
        [FirestoreProperty("closed")]
        public bool Closed { get; set; }
        [FirestoreProperty("closedAt")]
        public string ClosedAt { get; set; }
        [FirestoreProperty("createdAt")]
        public long CreatedAt { get; set; }
        [FirestoreProperty("guestQuantity")]
        public int GuestQuantity { get; set; }
        [FirestoreProperty("hostUserId")]
        public string HostUserId { get; set; }
        [FirestoreProperty("newUser")]
        public bool NewUser { get; set; }
        [FirestoreProperty("openedAt")]
        public string OpenedAt { get; set; }
        public bool Printed { get; set; }
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }
        [FirestoreProperty("tableCapacity")]
        public int TableCapacity { get; set; }

        [FirestoreProperty("tableNumberId")]
        public int TableNumberId { get; set; }
        [FirestoreProperty("tableOpenings")]
        public TableOpenings[] TableOpenings { get; set; }
        [FirestoreProperty("totalToPay")]
        public double TotalToPay { get; set; }
        [FirestoreProperty("updatedAt")]
        public long UpdatedAt { get; set; }

    }
}
