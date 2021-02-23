namespace Menoo.PrinterService.Infraestructure.Queues
{
    public sealed class PrintMessage
    {
        public string PrintEvent { get; set; }

        public string DocumentId { get; set; }

        public string TypeDocument { get; set; }

        public string SubTypeDocument { get; set; }

        public string Builder { get; set; }

        public string ExtraData { get; set; }
    }
}
