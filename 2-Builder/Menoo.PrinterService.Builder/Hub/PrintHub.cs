using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Builder.Hub
{
    public class PrintHub : Microsoft.AspNet.SignalR.Hub
    {
        private readonly IHubContext _hub;

        private static readonly Dictionary<string, string> _printers = new Dictionary<string, string>();

        private readonly EventLog _generalWriter;

        public PrintHub()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext("PrintHub");
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public override Task OnConnected()
        {
            _printers.Add(Context.ConnectionId, "");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        [HubMethodName("markAsPrinted")]
        public void MarkAsPrinted(string ticketId, string storeId, string printEvent)
        {
            var id = Guid.Parse(ticketId);
            using (var sqlServerContext = new PrinterContext())
            {
                sqlServerContext.MarkAsPrintedAsync(id, storeId, printEvent).GetAwaiter().GetResult();
            }
        }

        [HubMethodName("subscribe")]
        public void SubscribeToPrinterGroup(string connectionId, string clientPrinterId)
        {
            if (_printers.ContainsKey(connectionId))
            {
                _printers[connectionId] = clientPrinterId;
                this.Groups.Add(connectionId, clientPrinterId);
            }
        }

        public void SendToClient(Guid ticketId, string printEvent, Guid sectorId, string ticket, int copies)
        {
            _hub.Clients.Group(sectorId.ToString()).recieveTicket(ticketId.ToString(), printEvent, ticket, copies);
        }

        #region private methods
        private void Remove(string connectionId)
        {
            try
            {
                lock (_printers)
                {
                    foreach (var item in _printers.ToArray())
                    {
                        if (item.Key == connectionId)
                        {
                            _printers.Remove(item.Key);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _generalWriter.WriteEntry($"OnDisconnected():: Error al intentar desconectar el cliente de impresión. Detalles: {Environment.NewLine}{e.ToString()}", EventLogEntryType.Error);
            }
        }
        #endregion
    }
}
