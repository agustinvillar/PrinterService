using Menoo.PrinterService.Infraestructure.Queues;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IPublisherService
    {
        Task PublishAsync(string id, PrintMessage data);
    }
}
