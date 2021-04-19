using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Incremental")]
    public class Incremental
    {
        public long IncrementalId { get; set; }
    }
}