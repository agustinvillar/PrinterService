using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Interceptors
{
    public sealed class OnActionRecieve
    {
        private readonly TicketRepository _ticketRepository;

        public OnActionRecieve(TicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
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
                bool isPrinted = _ticketRepository.IsTicketPrinted(document).GetAwaiter().GetResult();
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
                _ticketRepository.SetPrintedAsync(document).GetAwaiter().GetResult();
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
