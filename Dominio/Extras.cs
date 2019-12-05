using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class Extras
    {
        [FirestoreProperty("id")]
        public string ID { get; set; }
        [FirestoreProperty("name")]
        public string Name { get; set; }
        [FirestoreProperty("price")]
        public double Price { get; set; }
        [FirestoreProperty("type")]
        public string Type { get; set; }
    }
}
