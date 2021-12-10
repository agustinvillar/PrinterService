using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IFormaterService
    {
        bool AllowFormat { get; }

        string Template { set; }

        Task PrintAsync();
    }
}
