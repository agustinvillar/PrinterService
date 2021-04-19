using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Invoice")]
    public class Invoice
    {
        public long InvoiceId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [Min(1)]
        public double Total { get; set; }
        [Required]
        public string User { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Serie { get; set; }
        [Required]
        public string Numero { get; set; }
        [Required]
        public DateTime CreatedAtDgi { get; set; }
        public CFETypeData CFETypeDataEnum { get; set; }
        [Required]
        public string DigestValue { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<AccountState> AccountStates { get; set; }
        public virtual Store Store { get; set; }

        public enum CFETypeData
        {
            ETicket = 101,
            ETicket_CreditNote = 102,
            EInvoice = 111,
            EInvoice_CreditNote = 112,
        }
    }
}