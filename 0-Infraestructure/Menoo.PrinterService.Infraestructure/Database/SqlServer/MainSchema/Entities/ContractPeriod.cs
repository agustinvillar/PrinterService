using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    public class ContractPeriod
    {
        public int ContractPeriodId { get; set; }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string ContractPeriodName { get; set; }
    }
}