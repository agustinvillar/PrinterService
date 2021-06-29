﻿using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
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
                        TypeDocument = PrintTypes.ORDER,
                        Builder = PrintBuilder.ORDER_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(messageQueue, false, true).GetAwaiter().GetResult();
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
                        TypeDocument = PrintTypes.ORDER,
                        Builder = PrintBuilder.ORDER_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(messageQueue).GetAwaiter().GetResult();
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
                        SubTypeDocument = SubOrderPrintTypes.ORDER_TA,
                        Builder = PrintBuilder.ORDER_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetOrderAsPrintedAsync(messageQueue).GetAwaiter().GetResult();
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
                    
                try
                {
                    var messageQueue = GetMessagePrintType(documentReference);
                    _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                    SetOrderAsRePrintedAsync(messageQueue, ticket).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    _generalWriter.WriteEntry($"OrderListener::OnRecieve(). Error desconocido al procesar el item a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                }
                finally
                {
                    Thread.Sleep(_delayTask);
                }
            }
        }
        #endregion

        #region private methods
        private PrintMessage GetMessagePrintType(DocumentSnapshot documentReference)
        {
            var printMessage = new PrintMessage
            {
                Builder = PrintBuilder.ORDER_BUILDER
            };
            var document = documentReference.ConvertTo<DocumentMessage>();
            switch (document.Event)
            {
                case "NEW_TABLE_ORDER":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.DocumentsId = document.EntityIdArray;
                    printMessage.PrintEvent = PrintEvents.NEW_TABLE_ORDER;
                    printMessage.TypeDocument = PrintTypes.ORDER;
                    printMessage.SubTypeDocument = SubOrderPrintTypes.ORDER_TABLE;
                    break;
                case "NEW_TAKE_AWAY":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.PrintEvent = PrintEvents.NEW_TAKE_AWAY;
                    printMessage.TypeDocument = PrintTypes.ORDER;
                    printMessage.SubTypeDocument = SubOrderPrintTypes.ORDER_TA;
                    break;
                case "ORDER_CANCELLED":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.DocumentsId = document.EntityIdArray;
                    printMessage.PrintEvent = PrintEvents.ORDER_CANCELLED;
                    printMessage.TypeDocument = PrintTypes.ORDER;
                    break;
            }
            return printMessage;
        }

        private List<string> GetOrdersToPrint(List<string> documentIds, bool isCreated, bool isCancelled) 
        {
            List<string> ticketsToPrint = null;
            using (var sqlServerContext = new PrinterContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documentIds, isCreated, isCancelled);
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

        private async Task SetOrderAsPrintedAsync(PrintMessage message, bool isNew = true, bool isCancelled = false)
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
