using Newtonsoft.Json;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business
{
    public static class Utils
    {
        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            var json = JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented);
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
    }
}
