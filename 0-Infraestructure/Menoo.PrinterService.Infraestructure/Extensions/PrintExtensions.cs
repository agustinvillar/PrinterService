using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Exceptions;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Extensions
{
    public static class PrintExtensions
    {
        public static List<PrintSettings> GetPrintSettings(this Store store, string printEvent)
        {
            List<PrintSettings> printSettings = new List<PrintSettings>();
            var queryResult = store.Sectors?.FindAll(f => f.PrintEvents.Contains(printEvent) && f.AllowPrinting);
            if (queryResult != null && queryResult.Count > 0)
            {
                printSettings.AddRange(queryResult);
            }
            return printSettings.OrderBy(o => o.Name).ToList();
        }

        public static Tuple<string, PrintMessage> GetMessagePrintType(this DocumentSnapshot documentReference)
        {
            Tuple<string, PrintMessage> printMessage = null;
            var message = new PrintMessage();
            var document = documentReference.ConvertTo<PrintEventMessage>();
            switch (document.Event)
            {
                #region orders
                case "NEW_TABLE_ORDER":
                    message.DocumentId = document.EntityId;
                    message.DocumentsId = document.EntityIdArray;
                    message.PrintEvent = PrintEvents.NEW_TABLE_ORDER;
                    message.TypeDocument = PrintTypes.ORDER;
                    message.SubTypeDocument = SubOrderPrintTypes.ORDER_TABLE;
                    message.Builder = PrintBuilder.ORDER_BUILDER;
                    break;
                case "NEW_TAKE_AWAY":
                    message.DocumentId = document.EntityId;
                    message.PrintEvent = PrintEvents.NEW_TAKE_AWAY;
                    message.TypeDocument = PrintTypes.ORDER;
                    message.SubTypeDocument = SubOrderPrintTypes.ORDER_TA;
                    message.Builder = PrintBuilder.ORDER_BUILDER;
                    break;
                case "ORDER_CANCELLED":
                    message.DocumentId = document.EntityId;
                    message.DocumentsId = document.EntityIdArray;
                    message.PrintEvent = PrintEvents.ORDER_CANCELLED;
                    message.TypeDocument = PrintTypes.ORDER;
                    message.Builder = PrintBuilder.ORDER_BUILDER;
                    break;
                #endregion
                #region tables
                case "TABLE_OPENED":
                    message.DocumentId = document.EntityId;
                    message.PrintEvent = PrintEvents.TABLE_OPENED;
                    message.TypeDocument = PrintTypes.TABLE;
                    message.Builder = PrintBuilder.TABLE_BUILDER;
                    break;
                case "TABLE_CLOSED":
                    message.DocumentId = document.EntityId;
                    message.PrintEvent = PrintEvents.TABLE_CLOSED;
                    message.TypeDocument = PrintTypes.TABLE;
                    message.Builder = PrintBuilder.TABLE_BUILDER;
                    break;
                case "REQUEST_PAYMENT":
                    message.DocumentId = document.EntityId;
                    message.PrintEvent = PrintEvents.REQUEST_PAYMENT;
                    message.TypeDocument = PrintTypes.TABLE;
                    message.Builder = PrintBuilder.TABLE_BUILDER;
                    break;
                #endregion
                #region bookings
                case "NEW_BOOKING":
                    message.DocumentId = document.EntityId;
                    message.DocumentsId = document.EntityIdArray;
                    message.PrintEvent = PrintEvents.NEW_BOOKING;
                    message.TypeDocument = PrintTypes.BOOKING;
                    message.Builder = PrintBuilder.BOOKING_BUILDER;
                    break;
                case "CANCELED_BOOKING":
                    message.DocumentId = document.EntityId;
                    message.DocumentsId = document.EntityIdArray;
                    message.PrintEvent = PrintEvents.CANCELED_BOOKING;
                    message.TypeDocument = PrintTypes.BOOKING;
                    message.Builder = PrintBuilder.BOOKING_BUILDER;
                    break;
                #endregion
                default:
                    var documentToReprint = GetReprintMessage(documentReference);
                    return documentToReprint;
            }
            if (message != null)
            {
                printMessage = new Tuple<string, PrintMessage>(documentReference.Id, message);
            }
            return printMessage;
        }

        public static Tuple<string, PrintMessage> GetReprintMessage(this DocumentSnapshot documentReference) 
        {
            documentReference.ToDictionary().TryGetValue("orderId", out object orderId);
            var messageQueue = new PrintMessage
            {
                DocumentId = orderId.ToString(),
                PrintEvent = PrintEvents.REPRINT_ORDER,
                TypeDocument = PrintTypes.ORDER,
                SubTypeDocument = string.Empty,
                Builder = PrintBuilder.ORDER_BUILDER
            };
            var printMessage = new Tuple<string, PrintMessage>(documentReference.Id, messageQueue);
            return printMessage;
        }

        public static PrintSettings SectorUnifiedTicket(this Store store) 
        {
            try
            {
                var sector = store.Sectors.FirstOrDefault(f => f.Id == store.UnifiedTicket.UnifiedTicketSectorId);
                if (sector == null) 
                {
                    return null;
                }
                return sector;
            }
            catch (Exception e) 
            {
                throw new UnifiedSectorException($"Existe un problema en la configuración de ticket unificado, para el restaurante {store.Name}-{store.Id}.", e);
            }
        }

        public static List<PrintSettings> RemoveDuplicates(this List<PrintSettings> sectors) 
        {
            var items = sectors.GroupBy(x => x.Name).Where(x => x.Count() == 1).Select(x => x.First()).ToList();
            return items;
        }
    }
}
