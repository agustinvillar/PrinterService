using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    class User
    {
        [FirestoreProperty("name")]
        public string Name { get; set; }
    }
}
