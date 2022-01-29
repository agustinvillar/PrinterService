using Menoo.Backend.Integrations.Messages;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        Task<PrintInfo> BuildAsync(PrintMessage data);
    }
}
