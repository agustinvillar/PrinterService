﻿using Google.Cloud.Firestore;
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

        public static EventLog ConfigureEventLog(IConfigurationManager config)
        {
            string sourceName = config.GetSetting("ServiceSourceName");
            if (!EventLog.SourceExists(config.GetSetting("DefaultLog")))
            {
                EventLog.CreateEventSource(sourceName, sourceName);
            }
            var generalWriter = new EventLog { Log = sourceName, Source = sourceName, EnableRaisingEvents = true };
            return generalWriter;
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
