using Menoo.PrinterService.Infraestructure.Exceptions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using NReco.ImageGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Westwind.RazorHosting;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class HtmlFormatService : IFormaterService
    {
        private readonly Dictionary<string, object> _viewData;

        private readonly EventLog _generalWriter;

        private readonly IFirebaseStorage _storageService;

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
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
            _storageService = GlobalConfig.DependencyResolver.Resolve<IFirebaseStorage>();
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
            var htmlToImageConv = new HtmlToImageConverter
            {
               Width = 350
            };
            var bytes = htmlToImageConv.GenerateImage(html, ImageFormat.Png);
            string urlImage = await _storageService.UploadAsync(bytes, $"ticket_{Guid.NewGuid().ToString()}");
            return urlImage;
        }
        
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
