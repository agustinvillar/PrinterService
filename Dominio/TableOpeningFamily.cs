using System;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class TableOpeningFamily
    {
        [FirestoreProperty("openedAt")]
        public string OpenedAt { get; set; }

        [FirestoreProperty("closedAt")]
        public string ClosedAt { get; set; }

        [FirestoreProperty("tableNumberId")]
        public int TableNumberId { get; set; }

        [FirestoreProperty("activeGuestQuantity")]

        public int ActiveGuestQuantity { get; set; }

        [FirestoreProperty("closed")]
        public bool Closed { get; set; }

        [FirestoreProperty("totalToPay")]
        public double TotalToPay { get; set; }

        [FirestoreProperty("openPrinted")]
        public bool OpenPrinted { get; set; }

        [FirestoreProperty("closedPrinted")]
        public bool ClosedPrinted { get; set; }

        public enum PRINTED_EVENT
        {
            OPENING,
            CLOSING
        }
    }
}
