using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [FirestoreData]
    public class Payment
    {
        [FirestoreProperty("paymentType")]
        public string PaymentType { get; set; }
        [FirestoreProperty("payMethod")]
        public string PaymentMethod { get; set; }
        public string TableOpeningId { get; set; }

    }
}
