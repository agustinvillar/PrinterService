using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.Entities;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interceptors;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Menoo.Printer.Listener.Tables
{
    [Handler]
    public class TablesListener : IFirebaseListener
    {
        private readonly FirestoreDb _firestoreDb;

        private readonly OnActionRecieve _interceptor;

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        public TablesListener(
            FirestoreDb firestoreDb,
            IPublisherService publisherService) 
        {
            _interceptor = new OnActionRecieve(PrintBuilder.TABLE_BUILDER);
            _firestoreDb = firestoreDb;
            _publisherService = publisherService;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("listener");
            _delayTask = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("listenerDelay"));
        }

        public void Listen()
        {
            //_firestoreDb.Collection("printEvent")
            //      .Listen(OnRecieve);
        }

        public override string ToString()
        {
            return PrintListeners.TABLE_LISTENER;
        }

        #region listeners
        private void OnClose(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnClose(). No se envió el cierre de mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnOpenFamily(Tuple<string, PrintMessage> ticket)
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnTakeAwayCreated(). No se envió la apertura de mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnRequestPayment(Tuple<string, PrintMessage> ticket) 
        {
            try
            {
                _publisherService.PublishAsync(ticket.Item1, ticket.Item2).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnRequestPayment(). No se envió la solicitud de pago de la mesa a la cola de impresión. {Environment.NewLine} Detalles: {e}{Environment.NewLine} {JsonConvert.SerializeObject(ticket.Item2, Formatting.Indented)}", EventLogEntryType.Error);
            }
        }

        private void OnRecieve(QuerySnapshot snapshot)
        {
            bool isEntry =_interceptor.OnEntry(snapshot);
            if (!isEntry) 
            {
                return;
            }
            var documentReference = snapshot.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
            var message = PrintExtensions.GetMessagePrintType(documentReference);
            if (message.Item2.PrintEvent == PrintEvents.TABLE_OPENED)
            {
                OnOpenFamily(message);
            }
            else if (message.Item2.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                OnClose(message);
            }
            else if (message.Item2.PrintEvent == PrintEvents.REQUEST_PAYMENT)
            {
                OnRequestPayment(message);
            }
            _interceptor.OnExit(snapshot);
            Thread.Sleep(_delayTask);
        }
        #endregion
    }
}
