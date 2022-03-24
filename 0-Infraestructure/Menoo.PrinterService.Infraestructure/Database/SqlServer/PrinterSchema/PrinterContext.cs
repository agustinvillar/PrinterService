using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using System;
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

        public DbSet<TicketHistory> TicketHistory { get; set; }

        public DbSet<PrinterConfiguration> PrinterConfiguration { get; set; }

        public DbSet<PrinterEvents> PrinterEvents { get; set; }

        public DbSet<PrinterStatus> PrinterStatus { get; set; }

        public DbSet<PrinterLog> PrinterEventSourcing { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("printer");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(PrinterContext).Assembly);
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

        public async Task SetPrintedAsync(PrintSettings sector, PrintInfo printData, string printEvent, string image)
        {
            var history = new TicketHistory
            {
                Id = Guid.NewGuid(),
                Copies = sector.Copies,
                PrinterName = sector.Printer,
                PrintEvent = printEvent,
                StoreId = printData.Store.Id,
                StoreName = printData.Store.Name,
                TicketImage = image,
                CreatedAt = DateTime.Now
            };
            this.TicketHistory.Add(history);
            await this.SaveChangesAsync();
        }
    }
}
