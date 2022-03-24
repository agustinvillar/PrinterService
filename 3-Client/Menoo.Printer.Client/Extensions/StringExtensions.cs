using Newtonsoft.Json;

namespace Menoo.PrinterService.Client.Extensions
{
    public static class StringExtensions
    {
        public static string ToJson(this object @object)
        {
            string json = JsonConvert.SerializeObject(@object, Formatting.Indented);
            return json;
        }

        public static T ToObject<T>(this string json)
        {
            var result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return result;
        }
    }
}
