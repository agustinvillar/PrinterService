using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Collections.Generic;

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
            return printSettings;
        }

        public static Tuple<string, PrintMessage> GetMessagePrintType(this DocumentSnapshot documentReference)
        {
            Tuple<string, PrintMessage> printMessage = null;
            var message = new PrintMessage();
            var document = documentReference.ConvertTo<DocumentMessage>();
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
                    break;
                #endregion
                default:
                    printMessage = null;
                    break;
            }
            if (message != null) 
            {
                printMessage = new Tuple<string, PrintMessage>(documentReference.Id, message);
            }
            return printMessage;
        }
    }
}
