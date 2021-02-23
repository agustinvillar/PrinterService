using System;

namespace Menoo.PrinterService.Infraestructure
{
    public class HandlerAttribute : Attribute
    {
        public int Order { get; set; }
    }
}
