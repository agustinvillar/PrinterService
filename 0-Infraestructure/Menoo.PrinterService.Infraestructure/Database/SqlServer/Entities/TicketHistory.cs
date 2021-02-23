using Menoo.PrinterService.Infraestructure.Enums;
using System;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.Entities
{
    public class TicketHistory
    {
        public TicketHistory() 
        {
            TicketHistorySettings = new List<TicketHistorySettings>();
        }

        public string Id { get; set; }

        public string ExternalId { get; set; }

        public string PrintEvent { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual List<TicketHistorySettings> TicketHistorySettings { get; set; }
    }
}
