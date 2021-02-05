using Menoo.PrinterService.Infraestructure.Queues;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        void Build(PrintMessage message);
    }
}
