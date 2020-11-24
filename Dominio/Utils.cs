using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Dominio
{
    public static class Utils
    {
        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var key in dict)
            {
                string propertyKey = key.Key;
                var property = type.GetProperty(propertyKey);
                if (property != null) 
                {
                    property.SetValue(obj, string.Empty + key.Value);
                }
            }
            return (T)obj;
        }
    }
}
