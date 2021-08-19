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

        public List<Tuple<string, PrintMessage>> GetItemsToPrint(List<Tuple<string, PrintMessage>> tickets, DateTime now, string printEvent)
        {
            var ticketsToPrint = new List<Tuple<string, PrintMessage>>();
            var printedTickets = GetTicketsPrinted(now, printEvent);
            foreach (var ticket in tickets)
            {
                bool documentExists = printedTickets.Contains(ticket.Item1);
                if (!documentExists) 
                {
                    ticketsToPrint.Add(ticket);
                }
            }
            return ticketsToPrint;
        }

        public List<string> GetItemsToRePrint(List<string> tickets, DateTime now) 
        {
            var ticketsToRePrint = new List<string>();
            var printedTickets = GetTicketsRePrinted(now);
            foreach (var ticket in tickets)
            {
                bool documentExists = printedTickets.Contains(ticket);
                if (!documentExists)
                {
                    ticketsToRePrint.Add(ticket);
                }
            }
            return ticketsToRePrint;
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
            string id = message.Item1;
            if (TicketHistory.Any(f => f.Id == id))
            {
                return;
            }
            var now = DateTime.Now;
            var history = new TicketHistory
            {
                Id = id,
                PrintEvent = message.Item2.PrintEvent,
                CreatedAt = DateTime.Now,
                DayCreatedAt = now.ToString("dd/MM/yyyy")
        };
            this.TicketHistory.Add(history);
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
        private List<string> GetTicketsPrinted(DateTime now, string printEvent)
        {
            string dateTimeNow = now.ToString("dd/MM/yyyy");
            List<string> ticketsPrinted = this.TicketHistory.ToList()
                .Where(f => f.DayCreatedAt == dateTimeNow)
                .Where(f => f.PrintEvent == printEvent)
                .Select(s => s.Id)
                .ToList();
  
            return ticketsPrinted;
        }

        private List<string> GetTicketsRePrinted(DateTime now)
        {
            string dateTimeNow = now.ToString("dd/MM/yyyy");
            List<string> ticketsPrinted = this.TicketHistory.ToList()
                .Where(f => f.DayCreatedAt == dateTimeNow)
                .Where(f => f.PrintEvent == PrintEvents.REPRINT_ORDER)
                .Select(s => s.Id )
                .ToList();
            return ticketsPrinted;
        }
        #endregion
    }
}
