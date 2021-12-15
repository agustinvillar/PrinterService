namespace Menoo.PrinterService.Infraestructure.Interfaces
{
    public interface IFormaterService
    {
        bool AllowFormat { get; }

        string Template { set; }

        string Create();
    }
}
