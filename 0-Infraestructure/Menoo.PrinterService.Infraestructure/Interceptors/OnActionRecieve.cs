using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Queues;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Linq;

namespace Menoo.PrinterService.Infraestructure.Interceptors
{
    [PSerializable]
    public sealed class OnActionRecieve : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            var documentReference = (QuerySnapshot)args.Arguments[0];
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                using (var sqlServerContext = new PrinterContext())
                {
                    if (sqlServerContext.IsTicketPrinted(document))
                    {
                        args.ReturnValue = args.MethodExecutionTag;
                        args.FlowBehavior = FlowBehavior.Return;
                    }
                }
            }
            else 
            {
                args.ReturnValue = args.MethodExecutionTag;
                args.FlowBehavior = FlowBehavior.Return;
            }
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var documentReference = (QuerySnapshot)args.Arguments[0];
            if (documentReference.Documents != null && documentReference.Documents.Count > 0)
            {
                var document = GetDocument(documentReference);
                using (var sqlServerContext = new PrinterContext())
                {
                    sqlServerContext.SetPrintedAsync(document).GetAwaiter().GetResult();
                }
            }
            else
            {
                args.ReturnValue = args.MethodExecutionTag;
                args.FlowBehavior = FlowBehavior.Return;
            }
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
