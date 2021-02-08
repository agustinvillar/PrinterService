using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders.Extensions
{
    public static class ItemExtensions
    {
        public static T GetPromotions<T>(this string json) where T : class
        {
            dynamic obj = JsonConvert.DeserializeObject(json);
            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj["promotions"]));
            return result;
        }

        public static List<SectorItem> GetPrintSector(this List<ItemOrderV2> items, ItemRepository repository)
        {
            List<SectorItem> sectorItems = new List<SectorItem>();
            foreach (var item in items)
            {
                var sectorByItem = repository.GetSectorItemById(item.Id).GetAwaiter().GetResult();
                if (sectorByItem != null)
                {
                    sectorItems.Add(sectorByItem);
                }
            }
            return sectorItems;
        }
    }
}
