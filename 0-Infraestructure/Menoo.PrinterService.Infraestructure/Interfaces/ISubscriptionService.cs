using Menoo.PrinterService.Infraestructure.Queues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ISubscriptionService
    {
        Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null);
    }
}
