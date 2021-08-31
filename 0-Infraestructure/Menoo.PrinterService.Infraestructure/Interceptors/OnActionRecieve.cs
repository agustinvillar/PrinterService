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

        public bool OnEntry(QuerySnapshot documentReference, bool isReprint = false)
        {
            bool validEntry = false;
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                if (document.Item2.Builder != _printEventType && !isReprint)
                {
                    validEntry = false;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER) 
                {
                    validEntry = false;
                }
                using (var sqlServerContext = new PrinterContext())
                {
                    if (sqlServerContext.IsTicketPrinted(document))
                    {
                        validEntry = false;
                    }
                }
                validEntry = true;
            }
            return validEntry;
        }

        public bool OnExit(QuerySnapshot documentReference, bool isReprint = false)
        {
            bool validEntry = false;
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                if (document.Item2.Builder != _printEventType && !isReprint)
                {
                    validEntry = false;
                }
                else if (isReprint && document.Item2.PrintEvent != PrintEvents.REPRINT_ORDER)
                {
                    validEntry = false;
                }
                using (var sqlServerContext = new PrinterContext())
                {
                    validEntry = true;
                    sqlServerContext.SetPrintedAsync(document).GetAwaiter().GetResult();
                }
            }
            return validEntry;
        }

        #region private methods
        private Tuple<string, PrintMessage> GetDocument(QuerySnapshot documentReference) 
        {
            var snapshot = documentReference.Single();
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
