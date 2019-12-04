using System;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class TableOpening
    {
        [FirestoreProperty("openedAt")]
        public string OpenedAt { get; set; }
        [FirestoreProperty("tableNumberId")]
        public int TableNumberId { get; set; }
        [FirestoreProperty("guestQuantity")]
        public int GuestQuantity { get; set; }
    }
}
