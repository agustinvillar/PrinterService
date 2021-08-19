using Menoo.PrinterService.Infraestructure.Enums;
using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistory
    {
        public string Id { get; set; }

        public string PrintEvent { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
