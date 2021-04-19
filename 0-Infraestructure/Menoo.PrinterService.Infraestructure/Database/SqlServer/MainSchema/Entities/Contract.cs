using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Contracts")]
    public class Contract
    {
        public long ContractId { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        public float FeePercent { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public double BaseFee { get; set; }
        [Required]
        public virtual ContractType Type { get; set; }
        [Required]
        public virtual ContractPeriod Period { get; set; }
        public virtual Store Store { get; set; }
    }
}