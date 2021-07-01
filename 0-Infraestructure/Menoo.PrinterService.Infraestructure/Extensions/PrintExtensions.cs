using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
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

        public static PrintMessage GetMessagePrintType(this DocumentSnapshot documentReference)
        {
            var printMessage = new PrintMessage();
            var document = documentReference.ConvertTo<DocumentMessage>();
            switch (document.Event)
            {
                #region orders
                case "NEW_TABLE_ORDER":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.DocumentsId = document.EntityIdArray;
                    printMessage.PrintEvent = PrintEvents.NEW_TABLE_ORDER;
                    printMessage.TypeDocument = PrintTypes.ORDER;
                    printMessage.SubTypeDocument = SubOrderPrintTypes.ORDER_TABLE;
                    printMessage.Builder = PrintBuilder.ORDER_BUILDER;
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
                #endregion
                #region tables
                case "TABLE_OPENED":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.PrintEvent = PrintEvents.TABLE_OPENED;
                    printMessage.TypeDocument = PrintTypes.TABLE;
                    printMessage.Builder = PrintBuilder.TABLE_BUILDER;
                    break;
                case "TABLE_CLOSED":
                    printMessage.DocumentId = document.EntityId;
                    printMessage.PrintEvent = PrintEvents.TABLE_CLOSED;
                    printMessage.TypeDocument = PrintTypes.TABLE;
                    printMessage.Builder = PrintBuilder.TABLE_BUILDER;
                    break;
                case "REQUEST_PAYMENT":
                    break;
                #endregion
                default:
                    printMessage = null;
                    break;
            }
            return printMessage;
        }
    }
}
