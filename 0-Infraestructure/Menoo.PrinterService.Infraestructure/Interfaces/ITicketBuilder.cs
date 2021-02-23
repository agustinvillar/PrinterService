using Menoo.PrinterService.Infraestructure.Queues;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        Task BuildAsync(PrintMessage data);
    }
}
