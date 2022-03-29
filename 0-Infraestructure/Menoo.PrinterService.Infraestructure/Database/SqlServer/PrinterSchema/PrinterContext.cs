using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema
{
    public class PrinterContext : DbContext
    {
        static PrinterContext()
        {
            System.Data.Entity.Database.SetInitializer<PrinterContext>(null);
        }

        public PrinterContext()
            : base("name=Microsoft.SQLServer.Menoo.ConnectionString")
        {
        }

        public PrinterContext(DbConnection dbConnection)
            : base(dbConnection, true)
        {
        }

        public DbSet<PrinterConfiguration> PrinterConfiguration { get; set; }

        public DbSet<PrinterEvents> PrinterEvents { get; set; }

        public DbSet<PrinterLog> PrinterEventSourcing { get; set; }

        public DbSet<PrinterStatus> PrinterStatus { get; set; }

        public DbSet<TicketHistory> TicketHistory { get; set; }

        public async Task<List<PrinterConfiguration>> GetPrinterConfigurationAsync(string storeId, string printEvent)
        {
            var printEventId = await this.PrinterEvents.FirstOrDefaultAsync(f => f.Value == printEvent);
            var configuration = await this.PrinterConfiguration.Where(f => f.StoreId == storeId && f.PrintEvents.Contains(printEventId.Id.ToString())).ToListAsync();
            return configuration;
        }

        public async Task MarkAsPrintedAsync(Guid ticketId, string storeId, string printEvent)
        {
            var record = await this.TicketHistory.FirstOrDefaultAsync(f => f.Id == ticketId);
            record.IsPrinted = true;
            record.UpdatedAt = DateTime.Now;
            var printerStatus = await this.PrinterStatus.FirstOrDefaultAsync(f => f.Id == (int)Status.Readed);
            var eventLog = new PrinterLog
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                PrintEvent = printEvent,
                Details = $"Ticket {ticketId.ToString()} impreso {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}",
                Status = printerStatus
            };
            this.PrinterEventSourcing.Add(eventLog);
            await this.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        public async Task WriteToHistory(
            Guid historyId,
            Store store,
            PrinterConfiguration sector,
            string printEvent,
            string image)
        {
            var history = new TicketHistory
            {
                Id = historyId,
                StoreId = store.Id,
                StoreName = store.Name,
                SectorName = sector.Name,
                PrintEvent = printEvent,
                Copies = sector.Copies,
                PrinterName = sector.Printer,
                TicketImage = image,
                IsPrinted = false,
                IsReprinted = false,
                CreatedAt = DateTime.Now
            };
            var printerStatus = await this.PrinterStatus.FirstOrDefaultAsync(f => f.Id == (int)Status.Delivered);
            var eventLog = new PrinterLog
            {
                Id = Guid.NewGuid(),
                StoreId = store.Id,
                PrintEvent = printEvent,
                Details = $"Ticket enviado al sector de impresión: {sector.Name}",
                Status = printerStatus,
                CreatedAt = DateTime.Now
            };
            this.PrinterEventSourcing.Add(eventLog);
            this.TicketHistory.Add(history);
            await this.SaveChangesAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("printer");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(PrinterContext).Assembly);
        }
    }
}
