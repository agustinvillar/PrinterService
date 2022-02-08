using System;

namespace Menoo.PrinterService.Infraestructure.Exceptions
{
    public class BadFormatTicketException : Exception
    {
        public BadFormatTicketException()
        {
        }

        public BadFormatTicketException(string message) : base(message)
        {
        }

        public BadFormatTicketException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
