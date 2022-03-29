using System;

namespace Menoo.PrinterService.Client.DTOs
{
    public class PrinterMessageDTO
    {
        public Guid Id { get; set; }

        public string Event { get; set; }

        public int Copies { get; set; }

        public string Status { get; set; }
    }
}
