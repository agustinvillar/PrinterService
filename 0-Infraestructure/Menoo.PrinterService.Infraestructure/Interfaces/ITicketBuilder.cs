using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Queues;
using System;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ITicketBuilder
    {
        Task<Tuple<string, string, Store>> BuildAsync(string id, PrintMessage data);
    }
}
