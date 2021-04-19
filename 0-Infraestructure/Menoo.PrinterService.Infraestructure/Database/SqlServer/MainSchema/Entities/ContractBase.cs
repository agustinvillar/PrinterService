using System.ComponentModel.DataAnnotations;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    public class ContractBase
    {
        public int ContractBaseId { get; set; }
        public virtual ContractType Type { get; set; }
        public virtual ContractPeriod Period { get; set; }
        [Required]
        public double BaseFee { get; set; }
        [Required]
        public double FeePercent { get; set; }
    }
}