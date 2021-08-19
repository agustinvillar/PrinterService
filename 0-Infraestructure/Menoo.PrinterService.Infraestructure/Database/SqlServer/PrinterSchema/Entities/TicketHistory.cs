using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistory
    {
        public string Id { get; set; }

        public string PrintEvent { get; set; }

        public string DayCreatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
