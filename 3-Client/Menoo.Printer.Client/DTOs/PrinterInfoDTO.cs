namespace Menoo.PrinterService.Client.DTOs
{
    public class PrinterInfoDTO
    {
        public string Name { get; set; }

        public string Status { get; set; }

        public bool IsDefault { get; set; }

        public bool IsNetwork { get; set; }

        public string PortName { get; set; }

        public int PrinterState { get; set; }

        public int PrinterStatus { get; set; }

        public string DeviceId { get; set; }
    }
}
