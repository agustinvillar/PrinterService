using System.Data.Entity;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema
{
    public class ModelConfiguration : DbConfiguration
    {
        public ModelConfiguration()
        {
            this.SetHistoryContext("System.Data.SqlClient",
                (connection, defaultSchema) => new MyHistoryContext(connection, defaultSchema));
        }
    }
}
