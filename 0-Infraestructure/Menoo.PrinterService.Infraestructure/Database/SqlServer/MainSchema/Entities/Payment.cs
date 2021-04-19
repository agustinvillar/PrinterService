using DataAnnotationsExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Payments")]
    public class Payment
    {
        public long PaymentId { get; set; }
        public string PaymentExternalId { get; set; }
        [Min(0)]
        public double TransactionAmount { get; set; }
        [Required]
        public string StoreId { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        public string CardBrand { get; set; }
        [MaxLength(100)]
        [Index(IsUnique = true)]
        [Required]
        public string EntityId { get; set; }
        public string PaymentReference { get; set; }
        public double Service { get; set; }
        public double FeeExternalAmount { get; set; }
        public double FeeInternalAmount { get; set; }
        public virtual Invoice Invoice { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public MethodEnum PaymentMethod { get; set; }
        public TypeEnum Type { get; set; }
        public StatusEnum Status { get; set; }
        public ConciliationEnum ConciliationStatus { get; set; }
        public ProviderEnum Provider { get; set; }
        [MaxLength(2)]
        public string ExpirationMonth { get; set; }
        [MaxLength(4)]
        public string ExpirationYear { get; set; }
        [StringLength(6)]
        public string FirstSixDigits { get; set; }
        [StringLength(4)]
        public string LastFourDigits { get; set; }
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }
        public virtual ICollection<PaymentRenglon> Renglones { get; set; }

        public enum ConciliationEnum
        {
            NOT_PAYED = 0,
            PAYED = 1
        }

        public enum ProviderEnum
        {
            NONE = 0,
            MERCADO_PAGO = 1,
            GEOPAY = 2
        }

        public enum TypeEnum
        {
            TAKE_AWAY = 0,
            TABLE_OPENING = 1,
            DEUDA = 2,
            BOOKING = 3
        }

        public enum StatusEnum
        {
            PENDING = 0,
            APPROVED = 1,
            AUTHORIZED = 2,
            IN_PROCESS = 3,
            IN_MEDIATION = 4,
            REJECTED = 5,
            CANCELLED = 6,
            REFUNDED = 7,
            CHARGED_BACK = 8
        }

        public enum MethodEnum
        {
            ACCOUNT_MONEY = 0,
            TICKET = 1,
            BANK_TRANSFER = 2,
            ATM = 3,
            CREDIT_CARD = 4,
            DEBIT_CARD = 5,
            PREPAID_CARD = 6,
            CASH = 7,
            NONE = 8,
            POS = 9,
            CUPON = 10,
            EXTRA = 11
        }
    }
}