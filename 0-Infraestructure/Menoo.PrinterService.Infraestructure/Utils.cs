using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure
{
    public static class Utils
    {
        public static string BeforeAt(string date, int minutes)
        {
            bool isValidDate = DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result);
            //Cuando es mesa cerrada, llega con otro formato.
            if (!isValidDate)
            {
                DateTime.TryParseExact(date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            return result.AddMinutes(minutes).ToString("yyyy/MM/dd HH:mm");
        }

        public static string Capitalize(this string text)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }

        public static List<object> GetBootstrapClasses()
        {
            var bootstrapObjects = new List<object>();
            Trace.Write("Discovering Bootstrap Classes: ");
            var types = GetMenooTypes();

            var handlers = (from t in types
                            let attribute = t.GetCustomAttribute<OnStartUpAttribute>()
                            where attribute != null
                            orderby attribute.Order
                            select t).ToList();

            Trace.WriteLine("OK");
            foreach (var classType in handlers)
            {
                Trace.WriteLine(string.Format("Bootstrapping {0} module", classType.GetCustomAttribute<OnStartUpAttribute>().Module));
                bootstrapObjects.Add(Activator.CreateInstance(classType));
            }

            return bootstrapObjects;
        }

        public static List<Type> GetMenooTypes()
        {
            return (from a in AppDomain.CurrentDomain.GetAssemblies().AsParallel()
                    where a.FullName.StartsWith("Menoo")
                    from t in a.GetLoadableTypes()
                    select t).ToList();
        }

        public static T GetObject<T>(this object element)
        {
            var json = JsonConvert.SerializeObject(element, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        public static async Task<List<Store>> GetStores(FirestoreDb db)
        {
            var stores = new List<Store>();
            var snapshot = await db.Collection("stores").GetSnapshotAsync();
            foreach (var item in snapshot.Documents)
            {
                var storeData = item.ToDictionary();
                var storeObject = storeData.GetObject<Store>();
                storeObject.Id = item.Id;
                stores.Add(storeObject);
            }
            return stores;
        }

        public static async Task<Store> GetStores(FirestoreDb db, string storeId)
        {
            var stores = await GetStores(db);
            return stores.SingleOrDefault(s => s != null && !string.IsNullOrEmpty(s.Id) && s.Id == storeId);
        }

        public static string GetTicketTemplate(string fileTemplate)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", fileTemplate + ".html");
            string template = File.ReadAllText(path);
            return template;
        }

        public static string GetTime(string dateTime)
        {
            var splitDateTime = dateTime.Split(' ');
            return splitDateTime[1];
        }

        public static void PreloadAssemblies()
        {
            Trace.Write("Getting Menoo Assemblies: ");
            List<Assembly> allAssemblies = new List<Assembly>();
            Assembly mainAssembly = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(mainAssembly.Location);

            var menooAssemblies = (from a in mainAssembly.GetReferencedAssemblies().AsParallel()
                                   where a.FullName.StartsWith("Menoo")
                                   select a).ToList();
            Trace.WriteLine("OK");
            Trace.Write("Preloading Menoo Assemblies: ");
            foreach (string dll in Directory.GetFiles(path, "Menoo*.dll"))
            {
                var fileName = Path.GetFileNameWithoutExtension(dll);
                if (!menooAssemblies.Any(a => a.Name == fileName) && Assembly.GetExecutingAssembly().GetName().Name != fileName)
                {
                    allAssemblies.Add(Assembly.LoadFile(dll));
                }
            }
            Trace.WriteLine("OK");
        }
    }
}
