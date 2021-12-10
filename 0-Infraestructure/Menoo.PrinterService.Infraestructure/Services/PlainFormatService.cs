using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class PlainFormatService : IFormaterService
    {
        private readonly Dictionary<string, string> _viewData;

        public bool AllowFormat
        {
            get { return true; }
        }

        public string Template { private get; set; }


        public PlainFormatService(Dictionary<string, string> viewData)
        {
            _viewData = viewData;
        }

        public async Task PrintAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
