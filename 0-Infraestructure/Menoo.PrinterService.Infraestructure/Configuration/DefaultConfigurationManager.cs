using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.PrinterService.Infraestructure.Configuration
{
    class DefaultConfigurationManager : IConfigurationManager
    {
        public string GetSetting(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(name);
        }
    }
}
