using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class SectorExtensions
    {
        public static List<SectorItem> GetPrintSector(List<ItemOrderV2> items, FirestoreDb db)
        {
            List<SectorItem> sectorItems = new List<SectorItem>();
            foreach (var item in items)
            {
                var sectorByItem = GetItemById(db, item.Id).GetAwaiter().GetResult();
                if (sectorByItem != null)
                {
                    sectorItems.Add(sectorByItem);
                }
            }
            return sectorItems;
        }

        #region private methods

        private static async Task<SectorItem> GetItemById(FirestoreDb db, string documentId)
        {
            SectorItem item = null;
            var resultQuery = await Utils.GetDocument(db, "items", documentId);
            var document = resultQuery.ToDictionary();
            if (document.ContainsKey("sectors"))
            {
                item = document.GetObject<SectorItem>();
                item.ItemId = documentId;
            }
            return item;
        }
        #endregion
    }
}
