using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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

        public static List<Tuple<Type, Type>> DiscoverListeners(Assembly assembly)
        {
            var listenerList = new List<Tuple<Type, Type>>();

            Trace.Write("Discovering listeners: ");
            var types = (from t in assembly.GetLoadableTypes()
                         select t).ToList();

            var handlers = (from t in types
                            let attributes = t.GetCustomAttributes(typeof(HandlerAttribute), true)
                            where attributes != null && attributes.Length > 0
                            orderby (attributes[0] as HandlerAttribute).Order
                            select t).ToList();

            Trace.WriteLine("OK");

            Trace.Write("Building listeners Table: ");
            foreach (var item in handlers)
            {
                foreach (var itype in item.GetInterfaces())
                {
                    var iListenerType = typeof(IFirebaseListener);
                    var iBuilderType = typeof(ITicketBuilder);
                    if (itype.Name == iListenerType.Name || itype == iBuilderType)
                    {
                        listenerList.Add(new Tuple<Type, Type>(itype, item));
                    }
                }
            }
            Trace.WriteLine("OK");

            return listenerList;
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

        public static List<Type> GetFirebaseListerners()
        {
            string typeWithNamespace = typeof(IFirebaseListener).Namespace;
            var type = typeof(IFirebaseListener);
            var listeners = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetLoadableTypes())
                .Where(filter => type.IsAssignableFrom(filter))
                .AsParallel()
                .ToList();
            return listeners.FindAll(filter => filter.FullName != typeWithNamespace);
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
            var json = JsonConvert.SerializeObject(element, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var result = JsonConvert.DeserializeObject<T>(json);
            return result;
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
