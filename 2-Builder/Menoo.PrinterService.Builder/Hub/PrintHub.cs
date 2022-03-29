using Menoo.PrinterService.Infraestructure;
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

        [HubMethodName("subscribe")]
        public void SubscribeToPrinterGroup(string storeId, string clientPrinterId)
        {
            if (_printers.ContainsKey(clientPrinterId))
            {
                _printers[clientPrinterId] = storeId;
                this.Groups.Add(clientPrinterId, storeId);
            }
        }

        public void SendToPrint(string storeId, string ticket, int copies, string printer)
        {
            _hub.Clients.Group(storeId).recieveTicket(ticket, copies, printer);
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
