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
        [FirestoreProperty("orders")]
        public Orders[] Orders { get; set; }
        [FirestoreProperty("userName")]
        public string UserName { get; set; }
        [FirestoreProperty("surcharge")]
        public int? Surcharge { get; set; }
        [FirestoreProperty("totalToPay")]
        public double? TotalToPay { get; set; }
        [FirestoreProperty("discounts")]
        public Discount[] Discounts { get; set; }

        [FirestoreData]
        public class Discount
        {
            [FirestoreProperty("Amount")]
            public double Amount { get; set; }
            [FirestoreProperty("Name")]
            public string Name { get; set; }
        }
    }
}
