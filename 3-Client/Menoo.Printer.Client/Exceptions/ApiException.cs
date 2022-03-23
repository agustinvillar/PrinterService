using System;
using System.Net;

namespace Menoo.PrinterService.Client.Exceptions
{
    public class ApiException : Exception
    {
        private readonly HttpStatusCode _statusCode;

        public ApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            _statusCode = statusCode;
        }

        public ApiException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            _statusCode = statusCode;
        }
    }
}
