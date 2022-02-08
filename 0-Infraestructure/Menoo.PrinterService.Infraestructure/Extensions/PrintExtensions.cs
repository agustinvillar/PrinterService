using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class PrintExtensions
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

        public static PrintSettings SectorUnifiedTicket(this Store store) 
        {
            try 
            {
                var sector = store.Sectors.FirstOrDefault(f => f.Id == store.UnifiedTicket.UnifiedTicketSectorId);
                if (sector == null) 
                {
                    return null;
                }
                return sector;
            }
            catch (Exception e) 
            {
                throw new UnifiedSectorException($"Existe un problema en la configuración de ticket unificado, para el restaurante {store.Name}-{store.Id}.", e);
            }
        }

        public static List<PrintSettings> RemoveDuplicates(this List<PrintSettings> sectors) 
        {
            var items = sectors.GroupBy(x => x.Name).Where(x => x.Count() == 1).Select(x => x.First()).ToList();
            return items;
        }
    }
}
