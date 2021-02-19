using Menoo.PrinterService.Infraestructure;
using System.ServiceProcess;

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
