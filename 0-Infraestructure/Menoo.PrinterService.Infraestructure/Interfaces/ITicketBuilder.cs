using Menoo.PrinterService.Infraestructure.Queues;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        Task BuildAsync(string id, PrintMessage data);
    }
}
