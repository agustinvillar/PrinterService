using Newtonsoft.Json;

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
    }
}
