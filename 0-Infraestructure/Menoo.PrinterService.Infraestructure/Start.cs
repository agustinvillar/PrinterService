using System;

namespace Menoo.PrinterService.Infraestructure
{
    [OnStartUp(Module = "Infrastructure", Order = Int32.MinValue)]
    public class Start
    {
        public Start()
        {

        }
    }
}
