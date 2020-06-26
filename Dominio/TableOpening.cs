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
        [FirestoreProperty("culteryPrice")]
        public int CulteryPrice { get; set; }

        public enum PRINTED_EVENT
        {
            OPENING,
            CLOSING
        }
    }
}
