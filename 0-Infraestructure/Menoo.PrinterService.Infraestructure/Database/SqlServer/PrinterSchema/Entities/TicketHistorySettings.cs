using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistorySettings
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public virtual TicketHistory TicketHistory { get; set; }

        public string TicketHistoryId { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Name, Value);
        }
    }
}
