﻿using Microsoft.AspNet.SignalR;

namespace Menoo.PrinterService.Builder.Hub
{
    public class PrintHub : Microsoft.AspNet.SignalR.Hub
    {
        private readonly IHubContext _hub;

        public PrintHub()
        {
            _hub = GlobalHost.ConnectionManager.GetHubContext("PrintHub");
        }

        public void SendToPrint(string ticket)
        {
            _hub.Clients.All.recieveTicket(ticket);
        }
    }
}
