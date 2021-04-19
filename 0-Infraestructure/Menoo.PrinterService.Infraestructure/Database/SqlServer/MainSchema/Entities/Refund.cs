using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Refunds")]
    public class Refund
    {
        public int RefundId { get; set; }
        public string ExternalId { get; }
        [Required]
        [Min(1)]
        public double Amount { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
    }
}