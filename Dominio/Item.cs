using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class Item
    {
        [FirestoreProperty("id")]
        public string Id { get; set; }
        [FirestoreProperty("name")]
        public string Name { get; set; }
        [FirestoreProperty("price")]
        public double Price { get; set; }
        [FirestoreProperty("quantity")]
        public double Quantity { get; set; }
        [FirestoreProperty("subTotal")]
        public double SubTotal { get; set; }
        [FirestoreProperty("guestComment")]
        public string GuestComment { get; set; }
        [FirestoreProperty("options")]
        public ItemOption[] Options { get; set; }

        [FirestoreData]
        public class ItemOption
        {
            public string Name { get; set; }
            public string Price { get; set; }
        }

    }
}
