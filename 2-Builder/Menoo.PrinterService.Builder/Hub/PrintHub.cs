using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Builder.Hub
{
    public class PrintHub : Microsoft.AspNet.SignalR.Hub
    {
        private readonly IHubContext _hub;

        private readonly Dictionary<string, string> _printers;

        public PrintHub()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext("PrintHub");
            _printers = new Dictionary<string, string>();
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
        public void SubscribeToPrinterGroup(string storeId, string clientConnectionId)
        {
            if (_printers.ContainsKey(clientConnectionId))
            {
                _printers[clientConnectionId] = storeId;
                this.Groups.Add(clientConnectionId, storeId);
            }
        }

        public void SendToPrint(string ticket, int copies, string printer)
        {
            _hub.Clients.All.recieveTicket(ticket, copies, printer);
        }

        #region private methods
        private void Remove(string connectionId)
        {
            lock (_printers)
            {
                foreach (var item in _printers)
                {
                    if (_printers.ContainsKey(connectionId))
                    {
                        _printers.Remove(item.Key);
                    }
                }
            }
        }
        #endregion
    }
}
