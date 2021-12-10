using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Westwind.RazorHosting;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class HtmlFormatService : IFormaterService
    {
        private readonly Dictionary<string, string> _viewData;

        public bool AllowFormat 
        {
            get { return true; }
        }

        public string Template { private get; set; }


        public HtmlFormatService(Dictionary<string, string> viewData) 
        {
            _viewData = viewData;
        }

        public async Task PrintAsync()
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            var host = new RazorFolderHostContainer
            {
                TemplatePath = fullPath,
                BaseBinaryFolder = AppDomain.CurrentDomain.BaseDirectory
            };
            host.Start();
            string html = host.RenderTemplate($"~/{this.Template}.cshtml", _viewData);
            host.Stop();
        }
    }
}
