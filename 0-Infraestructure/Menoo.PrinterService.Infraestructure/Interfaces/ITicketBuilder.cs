using Menoo.PrinterService.Infraestructure.Queues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        Task<List<PrintInfo>> BuildAsync(string id, PrintMessage data);
    }
}
