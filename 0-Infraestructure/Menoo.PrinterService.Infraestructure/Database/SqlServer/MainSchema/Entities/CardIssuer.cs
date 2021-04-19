using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("CardIssuer")]
    public class CardIssuer
    {
        public int CardIssuerId { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string CardBrand { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string IssuerName { get; set; }
        public DateTime Created { get; set; }
        public string Image { get; set; }
        public ICollection<AccountState> AccountStates { get; set; }
    }
}