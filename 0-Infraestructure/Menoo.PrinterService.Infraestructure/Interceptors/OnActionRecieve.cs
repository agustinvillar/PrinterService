using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Interceptors
{
    public sealed class OnActionRecieve
    {
        private readonly PrinterContext _printerContext;

        public OnActionRecieve(PrinterContext printerContext)
        {
            _printerContext = printerContext;
        }

        public bool OnEntry(QuerySnapshot documentReference, bool isReprint = false)
        {
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                if (!PrintEvents.EventExists(document.Item2.PrintEvent) && !isReprint)
                {
                    return false;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER)
                {
                    return false;
                }
                //using (var sqlServerContext = new PrinterContext())
                //{
                //    if (sqlServerContext.IsTicketPrinted(document))
                //    {
                //        return false;
                //    }
                //}
                bool isPrinted = _printerContext.IsTicketPrinted(document);
                if (isPrinted)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void OnExit(QuerySnapshot documentReference, bool isReprint = false)
        {

            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                if (!PrintEvents.EventExists(document.Item2.PrintEvent) && !isReprint)
                {
                    return;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER)
                {
                    return;
                }
                //using (var sqlServerContext = new PrinterContext())
                //{
                //    sqlServerContext.SetPrintedAsync(document).GetAwaiter().GetResult();
                //}
                _printerContext.SetPrintedAsync(document).GetAwaiter().GetResult();
            }
        }

        #region private methods
        private Tuple<string, PrintMessage> GetDocument(QuerySnapshot documentReference)
        {
            var snapshot = documentReference.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
            var document = PrintExtensions.GetMessagePrintType(snapshot);
            return document;
        }
        #endregion
    }
}
