using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities
{
    public class PrinterStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<PrinterLog> EntriesLog { get; set; }
    }
}
