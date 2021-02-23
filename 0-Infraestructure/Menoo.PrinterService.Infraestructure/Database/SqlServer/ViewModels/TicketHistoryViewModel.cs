namespace Menoo.PrinterService.Infraestructure.Database.SqlServer.ViewModels
{
    public class TicketHistoryViewModel
    {
        public string DocumentId { get; set; }

        public string IsCreatedPrinted { get; set; }

        public string IsCancelledPrinted { get; set; }
    }
}
