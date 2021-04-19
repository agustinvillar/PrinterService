using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("PhoneSmsVerification")]
    public class SmsVerification
    {
        public int SmsVerificationId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SistemaOperativo { get; set; }
        public string Code { get; set; }
        public bool UserValidate { get; set; }

    }
}