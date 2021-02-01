using Menoo.PrinterService.Infraestructure;
using System.ServiceProcess;

namespace Menoo.PrinterService.Listener
{
    static class Program
    {
        static void Main()
        {
            Boostrapper.Bootstrap();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PrinterListener()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
