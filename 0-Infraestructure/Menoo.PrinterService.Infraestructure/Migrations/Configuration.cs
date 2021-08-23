namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.PrinterContext>
    {
        public Configuration()
        {
            /*this.TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo("Data Source=menoo.cun3qa90azfx.us-east-2.rds.amazonaws.com;Initial Catalog=Menoo;User Id=Admin;Password=Menoo2019*", "System.Data.SqlClient");*/
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.PrinterContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
