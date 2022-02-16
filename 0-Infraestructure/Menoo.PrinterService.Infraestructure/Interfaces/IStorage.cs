using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IStorage
    {
        Task<string> UploadAsync(byte[] file);
    }
}
