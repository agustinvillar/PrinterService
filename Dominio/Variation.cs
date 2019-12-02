using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class Variation
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }
        [FirestoreProperty("name")]
        public string Name { get; set; }
        [FirestoreProperty("price")]
        public double Price { get; set; }
        [FirestoreProperty("checked")]
        public bool Checked { get; set; }
    }
}
