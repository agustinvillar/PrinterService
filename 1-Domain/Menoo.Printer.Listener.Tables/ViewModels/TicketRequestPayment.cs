using Menoo.PrinterService.Infraestructure.Database.SqlServer.ViewModels;

namespace Menoo.Printer.Listener.Tables
{
    public class TicketRequestPayment : TicketHistoryViewModel
    {
        public string IsRequestPaymentPrinted { get; set; }
    }
}
