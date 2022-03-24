using System;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class PrinterLog
    {
        public Guid Id { get; set; }

        public string StoreId { get; set; }

        public string PrintEvent { get; set; }

        public string Details { get; set; }

        public virtual PrinterStatus Status { get; set; }
    }

    public enum Status
    {
        Recieved = 0,

        Delivered = 1,

        Readed = 2,

        Error = 3
    }
}
