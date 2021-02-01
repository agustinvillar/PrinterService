using System.ServiceProcess;

namespace Menoo.PrinterService.Listener
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PrinterListener()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
