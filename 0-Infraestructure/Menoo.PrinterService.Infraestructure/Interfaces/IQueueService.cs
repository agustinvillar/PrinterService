using Menoo.PrinterService.Infraestructure.Queues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IQueueService
    {
        Task PublishAsync(Message data, Dictionary<string, string> extras = null);

        Task RecieveAsync(Message data, Dictionary<string, string> extras = null);
    }
}
