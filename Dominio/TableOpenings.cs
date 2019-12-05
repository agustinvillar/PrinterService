using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class TableOpenings
    {
        [FirestoreProperty("closed")]
        public bool Closed { get; set; }
        [FirestoreProperty("tableOpeningId")]
        public string TableOpeningId { get; set; }
        [FirestoreProperty("totalToPay")]
        public double TotalToPay { get; set; }
        [FirestoreProperty("userId")]
        public string UserId { get; set; }
        [FirestoreProperty("userName")]
        public string UserName { get; set; }
    }
}
