using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Client.DTOs
{
    public class ConfigurePrinterRequest
    {
        public int StoredId { get; set; }

        public string Name { get; set; }

        public bool? AllowLogo { get; set; }

        public bool AllowPrintQR { get; set; }

        public int Copies { get; set; }

        public string Printer { get; set; }

        public List<Guid> PrintEvents { get; set; }
    }
}
