using System;
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class TableOpening
    {
        public int CulteryPrice { get; set; }
        [FirestoreProperty("id")]
        public string Id { get; set; }
    }
}
