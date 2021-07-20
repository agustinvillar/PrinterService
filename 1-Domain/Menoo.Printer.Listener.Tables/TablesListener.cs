using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Extensions;
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
            //_firestoreDb.Collection("tableOpeningFamily")
            //      .OrderByDescending("openedAtNumber")
            //      .Limit(1)
            //      .Listen(OnOpenFamily);

            //_firestoreDb.Collection("tableOpeningFamily")
            //   .WhereEqualTo("closed", true)
            //   .Listen(OnClose);

            //_firestoreDb.Collection("tableOpeningFamily")
            //    .WhereEqualTo("closed", false)
            //    .Listen(OnRequestPayment);

            _firestoreDb.Collection("printEvent")
                  .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.TABLE_LISTENER;
        }

        #region listeners
        private void OnClose(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetTablesToPrint(tickets, true, false);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(ticket, false, true).GetAwaiter().GetResult();
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

        private void OnOpenFamily(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetTablesToPrint(tickets, true, false);
                foreach (var ticket in ticketsToPrint)
                {
                    try 
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(ticket).GetAwaiter().GetResult();
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

        #region request payment
        private void OnRequestPayment(List<Tuple<string, PrintMessage>> tickets) 
        {
            
        }

        /*private void OnRequestPayment(QuerySnapshot snapshot)
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
        }*/
        #endregion

        private void OnRecieve(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            if (snapshot.Documents.Count == 0)
            {
                return;
            }
            var tickets = snapshot.Documents
                                        .Where(filter => filter.Exists)
                                        .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                        .OrderByDescending(o => o.CreateTime)
                                        .Select(s => s.GetMessagePrintType())
                                        .ToList();
            var openTables = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.TABLE_OPENED);
            var closeTables = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.TABLE_CLOSED);
            var paymentsRequest = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.REQUEST_PAYMENT);
            if (openTables.Count() > 0)
            {
                OnOpenFamily(openTables);
            }
            if (closeTables.Count() > 0)
            {
                OnClose(closeTables);
            }
            if (paymentsRequest.Count() > 0)
            {

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
            using (var sqlServerContext = new PrinterContext())
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

        private List<Tuple<string, PrintMessage>> GetTablesToPrint(List<Tuple<string, PrintMessage>> documentIds, bool isCreated, bool isCancelled)
        {
            List<Tuple<string, PrintMessage>> ticketsToPrint = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documentIds, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private async Task SetTablesAsPrintedAsync(Tuple<string, PrintMessage> message, bool isNew = true, bool isCancelled = false, bool isRequestPayment = false)
        {
            string id = message.Item1;
            using (var sqlServerContext = new PrinterContext())
            {
                if (isNew)
                {
                    if (sqlServerContext.TicketHistory.Any(f => f.Id == id))
                    {
                        return;
                    }
                    var historyDetails = new List<TicketHistorySettings>()
                    {
                        new TicketHistorySettings{
                            TicketHistoryId = id,
                            Name = PrintProperties.IS_NEW_PRINTED,
                            Value = "true",
                            Id = Guid.NewGuid()
                        },
                        new TicketHistorySettings{
                            TicketHistoryId = id,
                            Name = PrintProperties.IS_CANCELLED_PRINTED,
                            Value = "false",
                            Id = Guid.NewGuid()
                        },
                        new TicketHistorySettings{
                            TicketHistoryId = id,
                            Name = PrintProperties.IS_REQUEST_PAYMENT,
                            Value = "false",
                            Id = Guid.NewGuid()
                        }
                    };

                    var history = new TicketHistory
                    {
                        Id = id,
                        PrintEvent = message.Item2.PrintEvent,
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
                    var ticketCreated = await sqlServerContext.TicketHistory.FirstOrDefaultAsync(f => f.Id == id);
                    ticketCreated.UpdatedAt = DateTime.Now;
                    var ticketCreatedSettings = await sqlServerContext.TicketHistorySettings.FirstOrDefaultAsync(f => f.TicketHistoryId == id && f.Name == PrintProperties.IS_CANCELLED_PRINTED);
                    ticketCreatedSettings.Value = "true";
                    sqlServerContext.SaveChanges();
                }
                if (isRequestPayment)
                {
                    var ticketCreated = await sqlServerContext.TicketHistory.FirstOrDefaultAsync(f => f.Id == id);
                    ticketCreated.UpdatedAt = DateTime.Now;
                    var ticketCreatedSettings = await sqlServerContext.TicketHistorySettings.FirstOrDefaultAsync(f => f.TicketHistoryId == id && f.Name == PrintProperties.IS_REQUEST_PAYMENT);
                    if (ticketCreatedSettings == null)
                    {
                        var historyDetails = new List<TicketHistorySettings>()
                        {
                            new TicketHistorySettings{
                                TicketHistoryId = id,
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
