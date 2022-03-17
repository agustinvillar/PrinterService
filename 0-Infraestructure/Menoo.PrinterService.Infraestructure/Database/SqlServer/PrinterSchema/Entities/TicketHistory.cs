using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class TicketHistory
    {
        public Guid Id { get; set; }

        public string StoreId { get; set; }

        public string StoreName { get; set; }

        public string SectorName { get; set; }

        public string PrintEvent { get; set; }

        public int Copies { get; set; }

        public string PrinterName { get; set; }

        public string TicketImage { get; set; }

        public bool IsPrinted { get; set; }

        public bool IsReprinted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
