﻿using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.ViewModels;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Database.SqlServer
{
    public class SqlServerContext : DbContext
    {
        public DbSet<Entities.TicketHistory> TicketHistory { get; set; }

        public DbSet<Entities.TicketHistorySettings> TicketHistorySettings { get; set; }

        static SqlServerContext()
        {
            System.Data.Entity.Database.SetInitializer<SqlServerContext>(null);
        }

        public SqlServerContext()
            : base("name=Microsoft.SQLServer.Print.ConnectionString")
        {
        }

        //internal SqlServerContext(string dbConnection)
        //    : base(dbConnection)
        //{
        //}

        public SqlServerContext(DbConnection dbConnection) 
            : base(dbConnection, true) 
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("printer");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(SqlServerContext).Assembly);
        }

        public List<string> GetItemsToPrint(List<string> documentIds, bool isCreated, bool isCancelled)
        {
            var ticketsToPrint = new List<string>();
            var printedTickets = GetTicketsPrintedAsync()
                            .GetAwaiter()
                            .GetResult();
            if (isCreated)
            {
                var printedTicketIds = printedTickets.Select(s => s.DocumentId).ToList();
                ticketsToPrint.AddRange((from c in documentIds
                                         where !printedTicketIds.Contains(c)
                                         select c));
            }
            else if (isCancelled)
            {
                foreach (var ticket in printedTickets)
                {
                    if (string.IsNullOrEmpty(ticket.IsCreatedPrinted) && string.IsNullOrEmpty(ticket.IsCancelledPrinted)) 
                    {
                        continue;
                    }
                    if (bool.Parse(ticket.IsCreatedPrinted) && !bool.Parse(ticket.IsCancelledPrinted) && documentIds.Contains(ticket.DocumentId))
                    {
                        ticketsToPrint.Add(ticket.DocumentId);
                    }
                }
            }
            return ticketsToPrint;
        }

        public List<string> GetItemsToRePrint(List<string> documentIds) 
        {
            var ticketsToRePrint = new List<string>();
            var printedTickets = GetTicketsRePrintedAsync()
                            .GetAwaiter()
                            .GetResult();
            var printedTicketIds = printedTickets.Select(s => s.DocumentId).ToList();
            ticketsToRePrint.AddRange((from c in documentIds
                                     where !printedTicketIds.Contains(c)
                                     select c));
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

        public async Task SetPrintedAsync(PrintMessage message, bool isNew = true, bool isCancelled = false)
        {
            if (isNew)
            {
                if (TicketHistory.Any(f => f.Id == message.DocumentId)) 
                {
                    return;
                }
                var historyDetails = new List<TicketHistorySettings>()
                {
                    new TicketHistorySettings{
                        TicketHistoryId = message.DocumentId,
                        Name = PrintProperties.IS_NEW_PRINTED,
                        Value = "true",
                        Id = Guid.NewGuid()
                    },
                    new TicketHistorySettings{
                        TicketHistoryId = message.DocumentId,
                        Name = PrintProperties.IS_CANCELLED_PRINTED,
                        Value = "false",
                        Id = Guid.NewGuid()
                    }
                };

                var history = new TicketHistory
                {
                    Id = message.DocumentId,
                    PrintEvent = message.PrintEvent,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ExternalId = string.Empty
                };
                this.TicketHistory.Add(history);
                this.TicketHistorySettings.AddRange(historyDetails);
                await this.SaveChangesAsync();
            }
            if (isCancelled)
            {
                var ticketCreated = await this.TicketHistory.FirstOrDefaultAsync(f => f.Id == message.DocumentId);
                ticketCreated.UpdatedAt = DateTime.Now;
                var ticketCreatedSettings = await this.TicketHistorySettings.FirstOrDefaultAsync(f => f.TicketHistoryId == message.DocumentId && f.Name == PrintProperties.IS_CANCELLED_PRINTED);
                ticketCreatedSettings.Value = "true";
                this.SaveChanges();
            }
        }

        public async Task SetRePrintedAsync(PrintMessage message, string documentId)
        {
            if (TicketHistory.Any(f => f.Id == documentId))
            {
                return;
            }
            var historyDetails = new List<TicketHistorySettings>()
                {
                    new TicketHistorySettings{
                        TicketHistoryId = documentId,
                        Name = PrintProperties.IS_REPRINTED,
                        Value = "true",
                        Id = Guid.NewGuid()
                    }
                };

            var history = new TicketHistory
            {
                Id = documentId,
                PrintEvent = message.PrintEvent,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ExternalId = string.Empty
            };
            this.TicketHistory.Add(history);
            this.TicketHistorySettings.AddRange(historyDetails);
            await this.SaveChangesAsync();
        }

        #region private methods
        private async Task<List<TicketHistoryViewModel>> GetTicketsPrintedAsync()
        {
            List<TicketHistoryViewModel> ticketsPrinted = await this.TicketHistorySettings.GroupBy(g => g.TicketHistoryId).Select(s => new TicketHistoryViewModel
            {
                DocumentId = s.Key,
                IsCancelledPrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_CANCELLED_PRINTED).Value,
                IsCreatedPrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_NEW_PRINTED).Value
            }).ToListAsync();
            return ticketsPrinted;
        }

        private async Task<List<TicketRePrinterViewModel>> GetTicketsRePrintedAsync()
        {
            List<TicketRePrinterViewModel> ticketsPrinted = await this.TicketHistorySettings.GroupBy(g => g.TicketHistoryId).Select(s => new TicketRePrinterViewModel
            {
                DocumentId = s.Key,
                IsRePrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_REPRINTED).Value
            }).ToListAsync();
            return ticketsPrinted;
        }
        #endregion
    }
}
