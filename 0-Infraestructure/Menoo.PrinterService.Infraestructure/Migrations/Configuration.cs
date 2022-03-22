namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.PrinterContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.PrinterContext context)
        {
            var printerStatus = new List<PrinterStatus>
            {
                new PrinterStatus { Id = (int)Status.Recieved, Name = "Recieved" },
                new PrinterStatus { Id = (int)Status.Delivered, Name = "Delivered" },
                new PrinterStatus { Id = (int)Status.Readed, Name = "Readed" },
                new PrinterStatus { Id = (int)Status.Error, Name = "Error" }
            };
            printerStatus.ForEach(item => context.PrinterStatus.Add(item));
            context.SaveChanges();
        }
    }
}
