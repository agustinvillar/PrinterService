using System;
using System.Collections.Generic;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class PrinterConfiguration
    {
        public Guid Id { get; set; }

        public string StoreId { get; set; }

        public string Name { get; set; }

        public bool AllowLogo { get; set; }

        public bool AllowPrintQR { get; set; }

        public int Copies { get; set; }

        public bool IsHtml { get; set; }

        public string Printer { get; set; }

        public string PrintEvents { get; set; }

        public List<Guid> PrintEventsId
        {
            get
            {
                var printIds = PrintEvents.Split(',').Select(s => Guid.Parse(s)).ToList();
                return printIds;
            }
        }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
