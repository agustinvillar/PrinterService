using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class PlainFormatService : IFormaterService
    {
        private readonly Dictionary<string, object> _viewData;

        public bool AllowFormat
        {
            get { return true; }
        }

        public string Template { private get; set; }


        public PlainFormatService(Dictionary<string, object> viewData)
        {
            _viewData = viewData;
        }

        public string Create()
        {
            throw new System.NotImplementedException();
        }
    }
}
