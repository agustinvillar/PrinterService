using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
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

        public DbSet<TicketHistoryDetail> TicketHistoryDetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("printer");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(PrinterContext).Assembly);
        }

        public bool IsTicketPrinted(Tuple<string, PrintMessage> message)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy");
            bool isExists = TicketHistory.AnyAsync(f => f.Id == message.Item1).GetAwaiter().GetResult();
            if (isExists) 
            {
                return true;
            }
            if (message.Item2.DocumentsId != null && message.Item2.DocumentsId.Count > 0)
            {
                var list = TicketHistory.Where(f => f.PrintEvent == message.Item2.PrintEvent)
                                        .Where(f => f.DayCreatedAt == now)
                                        .Select(s => s.TicketHistoryDetail)
                                        .FirstOrDefaultAsync()
                                        .GetAwaiter()
                                        .GetResult()
                                        .Select(s => s.EntityId)
                                        .ToList();
                isExists = list.Intersect(message.Item2.DocumentsId).Any();
            }
            else
            {
                isExists = TicketHistory.AnyAsync(f => f.TicketHistoryDetail.Select(s => s.EntityId).Contains(message.Item2.DocumentId) && f.PrintEvent == message.Item2.PrintEvent && f.DayCreatedAt == now).GetAwaiter().GetResult();
            }
            return isExists;
        }

        public bool IsTicketPrinted(QuerySnapshot documentSnapshot)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy");
            var snapshot = documentSnapshot.Single();
            bool isExists = TicketHistory.Any(f => f.Id == snapshot.Id);
            return isExists;
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
            bool isExists = await TicketHistory.AnyAsync(f => f.Id == message.Item1);
            if (isExists)
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
            this.TicketHistory.Add(history);
            if (message.Item2.DocumentsId != null && message.Item2.DocumentsId.Count > 0)
            {
                foreach (var document in message.Item2.DocumentsId)
                {
                    this.TicketHistoryDetail.Add(new Entities.TicketHistoryDetail
                    {
                        Id = Guid.NewGuid(),
                        EntityId = document,
                        TicketHistory = history
                    });
                }
            }
            else
            {
                this.TicketHistoryDetail.Add(new Entities.TicketHistoryDetail
                {
                    Id = Guid.NewGuid(),
                    EntityId = message.Item2.DocumentId,
                    TicketHistory = history
                });
            }
            await this.SaveChangesAsync();
        }

        public async Task UpdateAsync(string id, string documentHtml) 
        {
            bool isExists = await TicketHistory.AnyAsync(f => f.Id == id);
            if (!isExists)
            {
                throw new DbEntityValidationException($"Registro con id {id} no se encuentra registrado en la base de datos.");
            }
            var history = await TicketHistory.FirstOrDefaultAsync(f => f.Id == id);
            history.DocumentPrinted = documentHtml;
            await this.SaveChangesAsync();
        }
    }
}
