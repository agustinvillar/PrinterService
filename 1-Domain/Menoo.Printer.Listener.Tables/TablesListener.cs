using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.ViewModels;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Listener.Tables
{
    [Handler]
    public class TablesListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        private string _today;

        public TablesListener(
            FirestoreDb firestoreDb,
            IPublisherService publisherService) 
        {
            _firestoreDb = firestoreDb;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
            _delayTask = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("listenerDelay"));
        }

        public void Listen()
        {
            _firestoreDb.Collection("tableOpeningFamily")
                  .OrderByDescending("openedAtNumber")
                  .Limit(1)
                  .Listen(OnOpenFamily);

            _firestoreDb.Collection("tableOpeningFamily")
               .WhereEqualTo("closed", true)
               .Listen(OnClose);

            _firestoreDb.Collection("tableOpeningFamily")
                .WhereEqualTo("closed", false)
                .Listen(OnRequestPayment);
        }

        public override string ToString()
        {
            return "Tables.Listener";
        }

        #region listeners
        private void OnClose(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketTables = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.UpdateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetTablesToPrint(ticketTables, false, true);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.TABLE_CLOSED,
                        TypeDocument = PrintTypes.TABLE,
                        Builder = PrintBuilder.TABLE_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(messageQueue, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"TablesListener::OnClose(). No se envió el cierre de mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnClose(). Ha ocurrido un error al cerrar la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnOpenFamily(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                string currentId = snapshot.Documents.SingleOrDefault().Id;
                var ticketTables = new List<string> { snapshot.Documents.SingleOrDefault().Id };

                var ticketsToPrint = GetTablesToPrint(ticketTables, true, false);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.TABLE_OPENED,
                        TypeDocument = PrintTypes.TABLE,
                        Builder = PrintBuilder.TABLE_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(messageQueue).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnTakeAwayCreated(). No se envió la apertura de mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnOpenFamily(). Ha ocurrido un error al abrir la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnRequestPayment(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketTables = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.UpdateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetRequestPayment(ticketTables, snapshot);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(ticket, false, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"TablesListener::OnRequestPayment(). No se envió la solicitud de pago de mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnRequestPayment(). Ha ocurrido un error al enviar la solicitud de pago de la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }
        #endregion

        #region private methods

        private bool ContainsPayWithPOSProperty(IEnumerable<dynamic> tableOpenings)
        {
            int count = 0;
            foreach (var element in tableOpenings)
            {
                var item = (Dictionary<string, object>)element;
                if (item.ContainsKey("payWithPOS"))
                {
                    count++;
                }
            }
            return count > 0;
        }

        private List<PrintMessage> GetRequestPayment(List<string> documentIds, QuerySnapshot snapshot)
        {
            List<PrintMessage> ticketsToPrint = new List<PrintMessage>();
            List<TicketRequestPayment> ticketsPrinted = null;
            using (var sqlServerContext = new SqlServerContext())
            {
                ticketsPrinted = sqlServerContext.TicketHistorySettings.GroupBy(g => g.TicketHistoryId).Select(s => new TicketRequestPayment
                {
                    DocumentId = s.Key,
                    IsCancelledPrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_CANCELLED_PRINTED).Value,
                    IsCreatedPrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_NEW_PRINTED).Value,
                    IsRequestPaymentPrinted = s.FirstOrDefault(f => f.Name == PrintProperties.IS_REQUEST_PAYMENT).Value
                }).ToList();
            }
            foreach (var ticket in ticketsPrinted)
            {
                bool isExistsProperty = false;
                if (ticket.IsRequestPaymentPrinted != null) 
                {
                    isExistsProperty = bool.Parse(ticket.IsRequestPaymentPrinted);
                }
                if (string.IsNullOrEmpty(ticket.IsCreatedPrinted) && string.IsNullOrEmpty(ticket.IsCancelledPrinted)) 
                {
                    continue;
                }
                bool isNotPrinted = bool.Parse(ticket.IsCreatedPrinted) && !bool.Parse(ticket.IsCancelledPrinted) && !isExistsProperty && documentIds.Contains(ticket.DocumentId);
                var result = snapshot.Documents.FirstOrDefault(s => s.Id == ticket.DocumentId);
                if (result == null) 
                {
                    continue;
                }
                var document = result.ToDictionary();
                var tableOpenings = ((IEnumerable)document["tableOpenings"]).Cast<dynamic>();
                if (isNotPrinted && ContainsPayWithPOSProperty(tableOpenings))
                {
                    var message = new PrintMessage
                    {
                        DocumentId = ticket.DocumentId,
                        PrintEvent = PrintEvents.REQUEST_PAYMENT,
                        TypeDocument = PrintTypes.TABLE,
                        Builder = PrintBuilder.TABLE_BUILDER
                    };
                    bool payWithPos = document.GetObject<TableOpeningFamily>().TableOpenings.Any(f => f.PayWithPOS);
                    if (payWithPos)
                    {
                        message.SubTypeDocument = SubOrderPrintTypes.REQUEST_PAYMENT_POS;
                    }
                    else
                    {
                        message.SubTypeDocument = SubOrderPrintTypes.REQUEST_PAYMENT_CASH;
                    }
                    ticketsToPrint.Add(message);
                }
            }
            return ticketsToPrint;
        }

        private List<string> GetTablesToPrint(List<string> documentIds, bool isCreated, bool isCancelled)
        {
            List<string> ticketsToPrint = null;
            using (var sqlServerContext = new SqlServerContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documentIds, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private async Task SetTablesAsPrintedAsync(PrintMessage message, bool isNew = true, bool isCancelled = false, bool isRequestPayment = false)
        {
            using (var sqlServerContext = new SqlServerContext())
            {
                if (isNew)
                {
                    if (sqlServerContext.TicketHistory.Any(f => f.Id == message.DocumentId))
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
                        },
                        new TicketHistorySettings{
                            TicketHistoryId = message.DocumentId,
                            Name = PrintProperties.IS_REQUEST_PAYMENT,
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
                    sqlServerContext.TicketHistory.Add(history);
                    sqlServerContext.TicketHistorySettings.AddRange(historyDetails);
                    await sqlServerContext.SaveChangesAsync();
                }
                if (isCancelled)
                {
                    var ticketCreated = await sqlServerContext.TicketHistory.FirstOrDefaultAsync(f => f.Id == message.DocumentId);
                    ticketCreated.UpdatedAt = DateTime.Now;
                    var ticketCreatedSettings = await sqlServerContext.TicketHistorySettings.FirstOrDefaultAsync(f => f.TicketHistoryId == message.DocumentId && f.Name == PrintProperties.IS_CANCELLED_PRINTED);
                    ticketCreatedSettings.Value = "true";
                    sqlServerContext.SaveChanges();
                }
                if (isRequestPayment)
                {
                    var ticketCreated = await sqlServerContext.TicketHistory.FirstOrDefaultAsync(f => f.Id == message.DocumentId);
                    ticketCreated.UpdatedAt = DateTime.Now;
                    var ticketCreatedSettings = await sqlServerContext.TicketHistorySettings.FirstOrDefaultAsync(f => f.TicketHistoryId == message.DocumentId && f.Name == PrintProperties.IS_REQUEST_PAYMENT);
                    if (ticketCreatedSettings == null)
                    {
                        var historyDetails = new List<TicketHistorySettings>()
                        {
                            new TicketHistorySettings{
                                TicketHistoryId = message.DocumentId,
                                Name = PrintProperties.IS_REQUEST_PAYMENT,
                                Value = "true",
                                Id = Guid.NewGuid()
                            }
                        };
                        sqlServerContext.TicketHistorySettings.AddRange(historyDetails);
                    }
                    else 
                    {
                        ticketCreatedSettings.Value = "true";
                    }
                    sqlServerContext.SaveChanges();
                }
                await sqlServerContext.SetPrintedAsync(message, isNew, isCancelled);
            }
        }
        #endregion
    }
}
