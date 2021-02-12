using Google.Cloud.Firestore;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
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

        private readonly EventLog _generalWriter;

        private readonly IPublisherService _publisherService;

        private readonly int _delayTask;

        private string _today;

        public TablesListener(
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
            _firestoreDb.Collection("tableOpeningFamily")
                  .OrderByDescending("openedAtNumber")
                  .Limit(1)
                  .Listen(OnOpenFamily);

            _firestoreDb.Collection("tableOpeningFamily")
               .WhereEqualTo("closed", true)
               .Listen(OnClose);
        }

        public override string ToString()
        {
            return "Tables.Listener";
        }

        #region listeners
        private void OnClose(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketTables = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.UpdateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.UpdateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetTablesToPrint(ticketTables, false, true);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.TABLE_CLOSED,
                        TypeDocument = PrintTypes.TABLE,
                        Builder = PrintBuilder.TABLE_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(messageQueue, false, true).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"TablesListener::OnClose(). No se envió el cierre de mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnClose(). Ha ocurrido un error al cerrar la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }

        private void OnOpenFamily(QuerySnapshot snapshot)
        {
            _today = DateTime.UtcNow.ToString("dd/MM/yyyy");
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var ticketTables = snapshot.Documents
                                            .Where(filter => filter.Exists)
                                            .Where(filter => filter.CreateTime.GetValueOrDefault().ToDateTime().ToString("dd/MM/yyyy") == _today)
                                            .OrderByDescending(o => o.CreateTime)
                                            .Select(s => s.Id)
                                            .ToList();
                var ticketsToPrint = GetTablesToPrint(ticketTables, true, false);
                if (ticketsToPrint == null || ticketsToPrint.Count == 0)
                {
                    return;
                }
                foreach (var ticket in ticketsToPrint)
                {
                    var messageQueue = new PrintMessage
                    {
                        DocumentId = ticket,
                        PrintEvent = PrintEvents.TABLE_OPENED,
                        TypeDocument = PrintTypes.TABLE,
                        Builder = PrintBuilder.TABLE_BUILDER
                    };
                    try
                    {
                        _publisherService.PublishAsync(messageQueue).GetAwaiter().GetResult();
                        SetTablesAsPrintedAsync(messageQueue).GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        _generalWriter.WriteEntry($"OrderListener::OnTakeAwayCreated(). No se envió la apertura de mesa [{ticket}], a la cola de impresión.{Environment.NewLine} Detalles: {e}", EventLogEntryType.Error);
                    }
                    finally
                    {
                        Thread.Sleep(_delayTask);
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"TablesListener::OnOpenFamily(). Ha ocurrido un error al abrir la mesa. {Environment.NewLine} Detalles: {e.ToString()}", EventLogEntryType.Error);
            }
        }
        #endregion

        #region private methods
        private List<string> GetTablesToPrint(List<string> documentIds, bool isCreated, bool isCancelled)
        {
            List<string> ticketsToPrint = null;
            using (var sqlServerContext = new SqlServerContext())
            {
                ticketsToPrint = sqlServerContext.GetItemsToPrint(documentIds, isCreated, isCancelled);
            }
            return ticketsToPrint;
        }

        private async Task SetTablesAsPrintedAsync(PrintMessage message, bool isNew = true, bool isCancelled = false)
        {
            using (var sqlServerContext = new SqlServerContext())
            {
                await sqlServerContext.SetPrintedAsync(message, isNew, isCancelled);
            }
        }
        #endregion
    }
}
