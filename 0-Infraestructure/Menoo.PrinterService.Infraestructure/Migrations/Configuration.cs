namespace Menoo.PrinterService.Infraestructure.Migrations
{
    using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
    using System;
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
            var printEvents = new List<PrinterEvents>
            {
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Apertura de mesa", Value = "TABLE_OPENED" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Cierre de mesa", Value = "TABLE_CLOSED" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Orden de mesa", Value = "NEW_TABLE_ORDER" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Reserva", Value = "NEW_BOOKING" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Reserva cancelada", Value = "CANCELED_BOOKING" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Orden TakeAway", Value = "NEW_TAKE_AWAY" },
                new PrinterEvents { Id = Guid.NewGuid(), DisplayName = "Orden cancelada", Value = "ORDER_CANCELLED " },
                new PrinterEvents { Id= Guid.NewGuid(), DisplayName = "Solicitud de pago", Value = "REQUEST_PAYMENT" },
                new PrinterEvents { Id= Guid.NewGuid(), DisplayName = "Reimpresión de orden", Value = "REPRINT_ORDER" }
            };
            printerStatus.ForEach(item => context.PrinterStatus.Add(item));
            printEvents.ForEach(item => context.PrinterEvents.Add(item));
            context.SaveChanges();
        }
    }
}
