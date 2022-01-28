using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static List<Type> GetTypesFormaterServices() 
        {
            string typeWithNamespace = typeof(IFormaterService).Namespace;
            var type = typeof(IFormaterService);
            var fileServices = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetLoadableTypes())
                .Where(filter => type.IsAssignableFrom(filter))
                .AsParallel()
                .ToList();
            return fileServices.FindAll(filter => filter.FullName != typeWithNamespace);
        }
    }
}
