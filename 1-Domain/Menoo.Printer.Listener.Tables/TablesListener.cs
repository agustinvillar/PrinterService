using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Listener.Tables
{
    [Handler]
    public class TablesListener : IFirebaseListener
    {
        private readonly int _delayTask;

        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private string _today;

        public TablesListener(
            FirestoreDb firestoreDb,
            TableOpeningFamilyRepository tableOpeningFamilyRepository,
            IPublisherService publisherService) 
        {
            _firestoreDb = firestoreDb;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
            _delayTask = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("listenerDelay"));
        }

        public void Listen()
        {
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

        private void OnRequestPayment(List<Tuple<string, PrintMessage>> tickets) 
        {
            try
            {
                var ticketsToPrint = GetRequestPayment(tickets);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(ticket, false, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnRequestPayment(). No se envió la solicitud de pago de la mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnRequestPayment(). Ha ocurrido un error en la solicitud de pago de la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

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
            if (paymentsRequest.Count > 0) 
            {
                OnRequestPayment(paymentsRequest);
            }
        }
        #endregion

        #region private methods
        private List<Tuple<string, PrintMessage>> GetRequestPayment(List<Tuple<string, PrintMessage>> documents)
        {
            List<Tuple<string, PrintMessage>> ticketsToPrint = new List<Tuple<string, PrintMessage>>();
            List<TicketRequestPayment> ticketsPrinted = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsPrinted = sqlServerContext.TicketHistorySettings.Where(f => f.Name == PrintProperties.IS_REQUEST_PAYMENT).Select(s => new TicketRequestPayment
                {
                    Id = s.Id,
                    TicketHistoryId = s.TicketHistoryId,
                    IsRequestPaymentPrinted = s.Value
                }).ToList();
            }
            foreach (var document in documents)
            {
                if (!ticketsPrinted.Any(f => f.TicketHistoryId == document.Item1)) 
                {
                    ticketsToPrint.Add(document);
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
                if (sqlServerContext.TicketHistory.Any(f => f.Id == id))
                {
                    return;
                }
                var historyDetails = new List<TicketHistorySettings>();
                var history = new TicketHistory
                {
                    Id = id,
                    PrintEvent = message.Item2.PrintEvent,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ExternalId = string.Empty
                };
                sqlServerContext.TicketHistory.Add(history);
                if (isNew)
                {
                    historyDetails.Add(new TicketHistorySettings
                    {
                        TicketHistoryId = id,
                        Name = PrintProperties.IS_NEW_PRINTED,
                        Value = "true",
                        Id = Guid.NewGuid()
                    });
                }
                if (isCancelled)
                {
                    historyDetails.Add(new TicketHistorySettings
                    {
                        TicketHistoryId = id,
                        Name = PrintProperties.IS_CANCELLED_PRINTED,
                        Value = "true",
                        Id = Guid.NewGuid()
                    });
                }
                if (isRequestPayment)
                {
                    historyDetails.Add(new TicketHistorySettings
                    {
                        TicketHistoryId = id,
                        Name = PrintProperties.IS_REQUEST_PAYMENT,
                        Value = "true",
                        Id = Guid.NewGuid()
                    });
                }
                sqlServerContext.TicketHistorySettings.AddRange(historyDetails);
                await sqlServerContext.SaveChangesAsync();
            }
        }
        #endregion
    }
}
