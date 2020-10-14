using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class Store
    {
        [FirestoreProperty("id")]
        public string StoreId { get; set; }
        [FirestoreProperty("name")]
        public string Name { get; set; }
        [FirestoreProperty("allowPrinting")]
        public bool? AllowPrinting { get; set; }
    } 
}
