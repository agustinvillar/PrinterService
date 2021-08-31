using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistory
    {
        public TicketHistory() 
        {
            TicketHistoryDetail = new List<TicketHistoryDetail>();
        }

        public string Id { get; set; }

        public string PrintEvent { get; set; }

        public string DayCreatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DocumentPrinted { get; set; }

        public virtual List<TicketHistoryDetail> TicketHistoryDetail { get; set; }
    }

    public class TicketHistoryDetail
    {
        public Guid Id { get; set; }

        public string EntityId { get; set; }

        public string TicketHistoryId { get; set; }

        public virtual TicketHistory TicketHistory { get; set; }
    }
}
