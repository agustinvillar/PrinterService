using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("AccountState")]
    public class AccountState
    {
        public long AccountStateId { get; set; }
        public DateTime Date { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }
        public string Description { get; set; }
        public double Balance { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}