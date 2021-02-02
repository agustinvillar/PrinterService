namespace Menoo.PrinterService.Infraestructure.Queues
{
    public sealed class Message
    {
        public string PrintEvent { get; set; }

        public string DocumentId { get; set; }
    }
}
