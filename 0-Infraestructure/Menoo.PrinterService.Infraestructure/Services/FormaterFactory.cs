using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public static class FormaterFactory
    {
        public static IFormaterService Resolve(bool allowFormat, Dictionary<string, string> viewData, string template = "")
        {
            IFormaterService result = null;
            var services = TypeExtensions.GetTypesFormaterServices();
            foreach (var service in services)
            {
                var currentService = (IFormaterService)Activator.CreateInstance(service, viewData);
                if (allowFormat == currentService.AllowFormat)
                {
                    currentService.Template = template;
                    result = currentService;
                    break;
                }
                else
                {
                    result = currentService;
                    break;
                }
            }
            return result;
        }
    }
}
