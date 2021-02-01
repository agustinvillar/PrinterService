using Newtonsoft.Json;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class ItemExtensions
    {
        public static T GetPromotions<T>(this string json) where T : class
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj["promotions"]));
            return result;
        }

        //public static List<SectorItem> GetPrintSector(List<ItemOrderV2> items, FirestoreDb db)
        //{
        //    List<SectorItem> sectorItems = new List<SectorItem>();
        //    foreach (var item in items)
        //    {
        //        var sectorByItem = GetItemById(db, item.Id).GetAwaiter().GetResult();
        //        if (sectorByItem != null)
        //        {
        //            sectorItems.Add(sectorByItem);
        //        }
        //    }
        //    return sectorItems;
        //}

        #region private methods

        /*private static async Task<SectorItem> GetItemById(FirestoreDb db, string documentId)
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
        }*/
        #endregion
    }
}
