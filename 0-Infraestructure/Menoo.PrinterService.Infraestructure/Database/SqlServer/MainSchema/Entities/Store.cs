using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Stores")]
    public class Store
    {
        public int StoreId { get; set; }
        [Required]
        [MaxLength(100)]
        [Index("UI_Store", 0, IsUnique = true)]
        public string StoreIdAux { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Contract> Contratos { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        [Required]
        [MaxLength(100)]
        [Index("UI_Store", 1, IsUnique = true)]
        public string BusinessName { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Departament { get; set; }
        [Required]
        public string DocumentNumber { get; set; }
        public DocumentType DocumentTypeEnum { get; set; }
        [Required]
        public string Telephone { get; set; }
        [Required]
        public string Address { get; set; }
        public virtual ICollection<AccountState> AccountStates { get; set; }

        public enum DocumentType
        {
            RUC = 0,
            CI = 1,
            OTHER = 2,
            PASSPORT = 3,
            DNI = 4
        }
    }
}