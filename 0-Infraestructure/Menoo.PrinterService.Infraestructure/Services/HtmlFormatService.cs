using Menoo.PrinterService.Infraestructure.Interfaces;
using NReco.ImageGenerator;
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

        public string Create()
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            var host = new RazorFolderHostContainer
            {
                TemplatePath = fullPath,
                BaseBinaryFolder = AppDomain.CurrentDomain.BaseDirectory
            };
            host.Start();
            SetLogoAndStyle();
            string html = host.RenderTemplate($"~/{this.Template}.cshtml", _viewData);
            var htmlToImageConv = new HtmlToImageConverter
            {
                Width = 74,
                Zoom = 100
            };
            var jpegBytes = htmlToImageConv.GenerateImage(html, ImageFormat.Jpeg);
            string htmlBase64 = Convert.ToBase64String(jpegBytes);
            host.Stop();
            return htmlBase64;
        }

        #region private methods
        private void SetLogoAndStyle() 
        {
            string cssPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "assets", "print.css");
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "assets", "menoo-logo.png");
            string style = File.ReadAllText(cssPath);
            byte[] imageArray = File.ReadAllBytes(imagePath);
            string base64Image = $"data:image/png;base64, {Convert.ToBase64String(imageArray)}";
            _viewData.Add("style", style);
            _viewData.Add("logo", base64Image);
        }
        #endregion
    }
}
