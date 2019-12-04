using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dominio
{
    [FirestoreData]
    class TableOpening
    {
        [FirestoreProperty("guestQuantity")]
        public int GuestQuantity { get; set; }

        [FirestoreProperty("openedAt")]
        public string OpenedAt { get; set; }

        [FirestoreProperty("tableNumberId")]
        public int TableNumberId { get; set; }
    }
}
