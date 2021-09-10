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
        private readonly string _printEventType;

        public OnActionRecieve(string printEventType)
        {
            _printEventType = printEventType;
        }

        public OnActionRecieve() 
        {
        }

        public bool OnEntry(QuerySnapshot documentReference, bool isReprint = false)
        {
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                if (document.Item2.Builder != _printEventType && !isReprint)
                {
                    return false;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER) 
                {
                    return false;
                }
                using (var sqlServerContext = new PrinterContext())
                {
                    if (sqlServerContext.IsTicketPrinted(document))
                    {
                        return false;
                    }
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
                if (document.Item2.Builder != _printEventType && !isReprint)
                {
                    return;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER)
                {
                    return;
                }
                using (var sqlServerContext = new PrinterContext())
                {
                    sqlServerContext.SetPrintedAsync(document).GetAwaiter().GetResult();
                }
            }
        }

        #region private methods
        private Tuple<string, PrintMessage> GetDocument(QuerySnapshot documentReference) 
        {
            var snapshot = documentReference.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
            try
            {
                var document = PrintExtensions.GetMessagePrintType(snapshot);
                return document;
            }
            catch 
            {
                var documentToReprint = PrintExtensions.GetReprintMessage(snapshot);
                return documentToReprint;
            }
        }
        #endregion
    }
}
