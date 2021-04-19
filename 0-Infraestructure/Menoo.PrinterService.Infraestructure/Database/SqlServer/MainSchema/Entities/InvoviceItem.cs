using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("InvoviceItem")]
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        [Required]
        public string Description { get; set; }
        public TypeItem Type { get; set; }
        public DateTime DateCreated { get; set; }
        [Required]
        [Min(1)]
        public double Amount { get; set; }
        public enum TypeItem
        {
            PAYMENTS, FEE_BASE
        }
    }
}