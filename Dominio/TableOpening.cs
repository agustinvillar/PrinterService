using Google.Cloud.Firestore;

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

        [FirestoreProperty("propina")]
        public double? Tip { get; set; }

        [FirestoreProperty("discounts")]
        public Discount[] Discounts { get; set; }

        [FirestoreProperty("totalToPayWithSurcharge")]
        public double? TotalToPayWithSurcharge { get; set; }

        [FirestoreProperty("paidByOther")]
        public bool PaidByOther { get; set; }

        [FirestoreProperty("payingForAll")]
        public bool PayingForAll { get; set; }

        public bool PagoPorTodos => PayingForAll;
        public bool AlguienLePago => PaidByOther;
        public bool PagoPorElMismo => !PaidByOther && !PayingForAll;
    }
}
