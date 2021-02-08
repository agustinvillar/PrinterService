using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class PrintSettingsExtensions
    {
        public static List<PrintSettings> GetPrintSettings(this Store store, string printEvent)
        {
            List<PrintSettings> printSettings = new List<PrintSettings>();
            var queryResult = store.Sectors?.FindAll(f => f.PrintEvents.Contains(printEvent) && f.AllowPrinting);
            if (queryResult != null && queryResult.Count > 0)
            {
                printSettings.AddRange(queryResult);
            }
            return printSettings;
        }
    }
}
