using Google.Cloud.Firestore;
using Menoo.Printer.Listener.Orders.Constants;
using Menoo.Printer.Listener.Orders.ViewModels;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Listener.Orders
{
    [Handler]
    public class OrderListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        private string _today;

        public OrderListener(
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
            //Nuevo TA creado.
            _firestoreDb.Collection("orders")
               .WhereEqualTo("status", "pendiente")
               .Listen(OnTakeAwayCreated);

            //Nueva orden de reserva o mesa.
            _firestoreDb.Collection("orders")
               .WhereEqualTo("status", "preparando")
               .Listen(OnOrderCreated);

            //Orden cancelada.
            _firestoreDb.Collection("orders")
                .WhereEqualTo("status", "cancelado")
                .Listen(OnCancelled);
        }

        public override string ToString()
        {
            return "Order.Listener";
        }

        #region listeners
        private void OnCancelled(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketsOrders = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.UpdateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetOrdersToPrint(ticketsOrders, false, true);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.ORDER_CANCELLED,
                        TypeDocument = PrintTypes.ORDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrinted(messageQueue, false, true);
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnOrderCreated(). No se envió la orden [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnOrderCreated(). Ha ocurrido un error al capturar nuevas órdenes. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnOrderCreated(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketsOrders = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.CreateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetOrdersToPrint(ticketsOrders, true, false);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.NEW_ORDER,
                        TypeDocument = PrintTypes.ORDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrinted(messageQueue);
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnOrderCreated(). No se envió la orden [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnOrderCreated(). Ha ocurrido un error al capturar nuevas órdenes. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnTakeAwayCreated(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketsTakeAway = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.CreateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetOrdersToPrint(ticketsTakeAway, true, false);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.NEW_TAKE_AWAY,
                        TypeDocument = PrintTypes.ORDER,
                        SubTypeDocument = SubOrderPrintTypes.ORDER_TA
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrinted(messageQueue);
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnTakeAwayCreated(). No se envió la orden TA [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnTakeAwayCreated(). Ha ocurrido un error al capturar nuevos TA. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }
        #endregion

        #region private methods
        private List<string> GetOrdersToPrint(List<string> documentIds, bool isCreated, bool isCancelled)
        {
            var ticketsToPrint = new List<string>();
            var printedTickets = GetTicketsPrintedAsync()
                            .GetAwaiter()
                            .GetResult()
                            .Where(f => f.IsCreatedPrinted == "true")
                            .Where(f => f.IsCancelledPrinted == "false");
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
                    if (bool.Parse(ticket.IsCreatedPrinted) && documentIds.Contains(ticket.DocumentId))
                    {
                        ticketsToPrint.Add(ticket.DocumentId);
                    }
                }
            }
            return ticketsToPrint;
        }

        private async Task<List<TicketHistoryViewModel>> GetTicketsPrintedAsync()
        {
            List<TicketHistoryViewModel> ticketsPrinted = null;
            using (var sqlServerContext = new SqlServerContext())
            {
                ticketsPrinted = await sqlServerContext.TicketHistorySettings.GroupBy(g => g.TicketHistoryId).Select(s => new TicketHistoryViewModel
                {
                    DocumentId = s.Key,
                    IsCancelledPrinted = s.FirstOrDefault(f => f.Name == OrderProperties.IS_CANCELLED_PRINTED).Value,
                    IsCreatedPrinted = s.FirstOrDefault(f => f.Name == OrderProperties.IS_NEW_PRINTED).Value
                }).ToListAsync();
            }
            return ticketsPrinted;
        }

        private void SetOrderAsPrinted(PrintMessage message, bool isNew = true, bool isCancelled = false)
        {
            if (isNew)
            {
                var historyDetails = new List<TicketHistorySettings>()
                {
                    new TicketHistorySettings{
                        TicketHistoryId = message.DocumentId,
                        Name = OrderProperties.IS_NEW_PRINTED,
                        Value = "true",
                        Id = Guid.NewGuid()
                    },
                    new TicketHistorySettings{
                        TicketHistoryId = message.DocumentId,
                        Name = OrderProperties.IS_CANCELLED_PRINTED,
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
                using (var sqlServerContext = new SqlServerContext())
                {
                    sqlServerContext.TicketHistory.Add(history);
                    sqlServerContext.TicketHistorySettings.AddRange(historyDetails);
                    sqlServerContext.SaveChanges();
                }
            }
            if (isCancelled) 
            {
                using (var sqlServerContext = new SqlServerContext())
                {
                    var ticketCreated = sqlServerContext.TicketHistorySettings.FirstOrDefault(f => f.TicketHistoryId == message.DocumentId && f.Name == OrderProperties.IS_CANCELLED_PRINTED);
                    ticketCreated.Value = "true";
                    sqlServerContext.SaveChanges();
                }
            }
        }
        #endregion
    }
}
