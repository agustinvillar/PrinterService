using CoreHtmlToImage;
using Menoo.PrinterService.Infraestructure.Exceptions;
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
        private readonly Dictionary<string, object> _viewData;

        private readonly IStorage _storageService;

        private const int TICKET_WIDTH = 300;

        public bool AllowFormat
        {
            get { return true; }
        }

        public string Template { private get; set; }

        public HtmlFormatService(Dictionary<string, object> viewData)
        {
            if (viewData == null || viewData.Count == 0)
            {
                throw new ArgumentException("viewData");
            }
            _viewData = viewData;
            _storageService = GlobalConfig.DependencyResolver.Resolve<IStorage>();
        }

        public string Create()
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            var host = new RazorFolderHostContainer
            {
                TemplatePath = fullPath,
                BaseBinaryFolder = AppDomain.CurrentDomain.BaseDirectory
            };
            host.AddAssemblyFromType(typeof(PrintInfo));
            host.Start();
            SetLogoAndStyle();
            string html = host.RenderTemplate($"~/{this.Template}", _viewData);
            if (string.IsNullOrEmpty(html))
            {
                throw new BadFormatTicketException($"HtmlFormatService::Create(). Error generando el HTML. Detalles: {host.ErrorMessage}");
            }
            string urlImage = GetImageUrlAsync(html).GetAwaiter().GetResult();
            host.Stop();
            return urlImage;
        }

        #region private methods
        private async Task<string> GetImageUrlAsync(string html)
        {
            var converter = new HtmlConverter();
            var bytes = converter.FromHtmlString(html, TICKET_WIDTH, CoreHtmlToImage.ImageFormat.Png, 100);
            string urlImage = await _storageService.UploadAsync(bytes);
            return urlImage;
        }

        private void SetLogoAndStyle()
        {
            string cssPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "assets", "print.css");
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "assets", "menoo-logo.png");
            string style = File.ReadAllText(cssPath);
            byte[] imageArray = File.ReadAllBytes(imagePath);
            string base64Image = $"data:image/png;base64, {Convert.ToBase64String(imageArray)}";
            if (!_viewData.ContainsKey("style"))
            {
                _viewData.Add("style", style);
            }
            if (!_viewData.ContainsKey("logo"))
            {
                _viewData.Add("logo", base64Image);
            }
        }
        #endregion
    }
}
