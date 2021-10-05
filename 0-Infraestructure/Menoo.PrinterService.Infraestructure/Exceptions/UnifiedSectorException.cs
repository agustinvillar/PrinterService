using System;

namespace Menoo.PrinterService.Infraestructure.Exceptions
{
    public class UnifiedSectorException : Exception
    {
        public UnifiedSectorException()
        {
        }

        public UnifiedSectorException(string message) : base(message)
        {
        }

        public UnifiedSectorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
