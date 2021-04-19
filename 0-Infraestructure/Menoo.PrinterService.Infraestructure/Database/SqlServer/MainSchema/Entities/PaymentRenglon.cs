using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("PaymentRenglon")]
    public class PaymentRenglon
    {
        public long PaymentRenglonId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Signo => Type == PaymentRenglonType.DISCOUNT ? -1 : 1;
        public PaymentRenglonType Type { get; set; }

        public enum PaymentRenglonType
        {
            SURCHARGE = 0,
            DISCOUNT = 1,
            NORMAL = 2,
            IVA = 3,
            CUPON = 4,
            EXTRA = 5
        }
    }
}