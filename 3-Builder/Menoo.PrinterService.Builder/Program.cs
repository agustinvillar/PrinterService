using Menoo.PrinterService.Infraestructure;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;

namespace Menoo.PrinterService.Builder
{
    static class Program
    {
        static void Main()
        {
            Bootstrapper.Bootstrap();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Builder()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
