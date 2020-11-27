using System.Linq;
using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Entities
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

        [FirestoreProperty("categoryId")]
        public string CategoryId { get; set; }

        [FirestoreProperty("total")]
        public double Total { get; set; }

        public double PriceToTicket => Total;

        public CategoryStore CategoryStore => Store.CategoryStore.SingleOrDefault(s => s.Id == CategoryId);
        public Store Store { get; set; }

        [FirestoreData]
        public class ItemOption
        {
            [FirestoreProperty("name")]
            public string Name { get; set; }
            [FirestoreProperty("price")]
            public double Price { get; set; }
        }

    }
}
