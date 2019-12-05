using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class ActivityLog
    {
        [FirestoreProperty("date")]
        public string Date { get; set;  }
        [FirestoreProperty("message")]
        public string Message { get; set; }
        [FirestoreProperty("type")]
        public string Type { get; set; }
    }
}
