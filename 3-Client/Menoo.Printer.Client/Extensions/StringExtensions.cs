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
    }
}
