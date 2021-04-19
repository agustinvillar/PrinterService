using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities
{
    [Table("Logs")]
    public class Log
    {
        public int LogId { get; set; }
        public DateTime Date { get; set; }
        public string Error { get; set; }
        public string UserId { get; set; }
        public string Page { get; set; }
        public string Function { get; set; }
        public ModuleEnum Module { get; set; }

        public enum ModuleEnum
        {
            SIN_ESPECIFICAR = 0,
            MOBILE = 1,
            ADMIN = 2
        }
    }
}