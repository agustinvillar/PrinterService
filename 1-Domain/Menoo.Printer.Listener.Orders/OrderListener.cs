using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
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
            ////Nuevo TA creado.
            //_firestoreDb.Collection("orders")
            //   .WhereEqualTo("status", "pendiente")
            //   .Listen(OnTakeAwayCreated);

            ////Nueva orden de reserva o mesa.
            //_firestoreDb.Collection("orders")
            //   .WhereEqualTo("status", "preparando")
            //   .Listen(OnOrderCreated);

            ////Orden cancelada.
            //_firestoreDb.Collection("orders")
            //    .WhereEqualTo("status", "cancelado")
            //    .Listen(OnCancelled);

            //Reimpresión de orden
            _firestoreDb.Collection("rePrint")
                .Listen(RePrintOrder);

            _firestoreDb.Collection("printEvent")
                  .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.ORDER_LISTENER;
        }

        #region listeners
        private void OnCancelled(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetOrdersToPrint(tickets, false, true);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(ticket, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnCancelled(). No se envió la orden [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::OnCancelled(). Ha ocurrido un error al cancelar órdenes. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnOrderCreated(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetOrdersToPrint(tickets, true, false);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(ticket).GetAwaiter().GetResult();
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

        private void OnTakeAwayCreated(List<Tuple<string, PrintMessage>> tickets)
        {
            try
            {
                var ticketsToPrint = GetOrdersToPrint(tickets, true, false);
                foreach (var ticket in ticketsToPrint)
                {
                    try
                    {
                        _publisherService.PublishAsync(ticket.Item2).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(ticket).GetAwaiter().GetResult();
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

        private void RePrintOrder(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var tickets = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.CreateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetOrdersReToPrint(tickets);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var documentReference = snapshot.Documents.FirstOrDefault(f => f.Id == ticket && f.Exists);
                    if (documentReference == null) 
                    {
                        continue;
                    }
                    documentReference.ToDictionary().TryGetValue("orderId", out object orderId);
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = orderId.ToString(),
                        PrintEvent = PrintEvents.REPRINT_ORDER,
                        TypeDocument = PrintTypes.ORDER,
                        SubTypeDocument = string.Empty,
                        Builder = PrintBuilder.ORDER_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsRePrintedAsync(messageQueue, ticket).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::RePrintOrder(). No se envió la orden [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OrderListener::RePrintOrder(). Ha ocurrido un error al reimprimir una orden. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
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
            var newTableOrderTickets = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.NEW_TABLE_ORDER);
            var newTakeAwayTickets = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.NEW_TAKE_AWAY);
            var orderCancelledTickets = tickets.FindAll(f => f.Item2.PrintEvent == PrintEvents.ORDER_CANCELLED);
            if (newTableOrderTickets.Count() > 0) 
            {
                OnOrderCreated(newTableOrderTickets);
            }
            if (newTakeAwayTickets.Count() > 0) 
            {
                OnTakeAwayCreated(newTakeAwayTickets);
            }
            if (orderCancelledTickets.Count() > 0) 
            {
                OnCancelled(orderCancelledTickets);
            }
        }
        #endregion

        #region private methods

        private List<Tuple<string, PrintMessage>> GetOrdersToPrint(List<Tuple<string, PrintMessage>> tickets, bool isCreated, bool isCancelled) 
        {
            List<Tuple<string, PrintMessage>> ticketsToPrint = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(tickets, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private List<string> GetOrdersReToPrint(List<string> documentIds)
        {
            List<string> ticketsToRePrint = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsToRePrint = sqlServerContext.GetItemsToRePrint(documentIds);
            }
            return ticketsToRePrint;
        }

        private async Task SetOrderAsPrintedAsync(Tuple<string, PrintMessage> message, bool isNew = true, bool isCancelled = false)
        {
            using (var sqlServerContext = new PrinterContext())
            {
                await sqlServerContext.SetPrintedAsync(message, isNew, isCancelled);
            }
        }

        private async Task SetOrderAsRePrintedAsync(PrintMessage message, string documentId)
        {
            using (var sqlServerContext = new PrinterContext())
            {
                await sqlServerContext.SetRePrintedAsync(message, documentId);
            }
        }
        #endregion
    }
}
