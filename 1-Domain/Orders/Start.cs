using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Database.Firebase;
using Menoo.PrinterService.Infraestructure.Database.SqlServer;
using Menoo.PrinterService.Infraestructure.Repository;
using System.Diagnostics;

namespace Menoo.Printer.Orders
{
    [OnStartUp(Module = "Orders", Order = 1)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            
        }
    }
}
