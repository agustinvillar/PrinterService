using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IFirebaseStorage
    {
        Task<string> UploadAsync(byte[] file, string fileName);
    }
}
