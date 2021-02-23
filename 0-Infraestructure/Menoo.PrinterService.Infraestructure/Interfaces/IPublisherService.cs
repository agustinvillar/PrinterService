using Menoo.PrinterService.Infraestructure.Queues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IPublisherService
    {
        Task PublishAsync(PrintMessage data, Dictionary<string, string> extras = null);
    }
}
