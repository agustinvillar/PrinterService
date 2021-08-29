using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
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
        public DbSet<TicketHistory> TicketHistory { get; set; }

        public DbSet<TicketHistoryDetail> TicketHistoryDetail { get; set; }

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

        public async Task SetPrintedAsync(Tuple<string, PrintMessage> message)
        {
            if (TicketHistory.Any(f => f.Id == message.Item1)) 
            {
                return;
            }
            var now = DateTime.Now;
            var history = new TicketHistory
            {
                Id = message.Item1,
                PrintEvent = message.Item2.PrintEvent,
                CreatedAt = DateTime.Now,
                DayCreatedAt = now.ToString("dd/MM/yyyy")
            };
            var historyDetails = new TicketHistoryDetail 
            {
                
            };
            await this.SaveChangesAsync();
        }

        public async Task SetRePrintedAsync(PrintMessage message, string documentId)
        {
            if (TicketHistory.Any(f => f.Id == documentId))
            {
                return;
            }
            var now = DateTime.Now;
            var history = new TicketHistory
            {
                Id = documentId,
                PrintEvent = message.PrintEvent,
                CreatedAt = DateTime.Now,
                DayCreatedAt = now.ToString("dd/MM/yyyy")
            };
            this.TicketHistory.Add(history);
            await this.SaveChangesAsync();
        }

        #region private methods
        
        #endregion
    }
}
