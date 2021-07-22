using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema.ViewModels;
using System;

namespace Menoo.Printer.Listener.Tables
{
    public class TicketRequestPayment
    {
        public Guid Id { get; set; }

        public string TicketHistoryId { get; set; }

        public string IsRequestPaymentPrinted { get; set; }
    }
}
