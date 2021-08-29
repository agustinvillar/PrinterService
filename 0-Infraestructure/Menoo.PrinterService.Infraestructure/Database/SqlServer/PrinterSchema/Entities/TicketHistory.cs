using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistory
    {
        public string Id { get; set; }

        public string PrintEvent { get; set; }

        public string DayCreatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DocumentPrinted { get; set; }

        public virtual TicketHistoryDetail TicketHistoryDetail { get; set; }
    }

    public class TicketHistoryDetail
    {
        public Guid Id { get; set; }

        public string OrderId { get; set; }

        public virtual TicketHistory TicketHistory { get; set; }
    }
}
