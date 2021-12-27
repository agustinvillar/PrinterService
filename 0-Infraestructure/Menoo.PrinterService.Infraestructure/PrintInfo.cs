using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure
{
    public class PrintInfo
    {
        public string BeforeAt { get; set; }

        public Dictionary<string, object> Content { get; set; }

        public string Template { get; set; }

        public Store Store { get; set; }
    }
}
