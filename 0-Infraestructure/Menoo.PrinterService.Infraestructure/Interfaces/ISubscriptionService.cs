using Menoo.Backend.Integrations.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface ISubscriptionService
    {
        Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null);
    }
}
