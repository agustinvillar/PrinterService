using Menoo.PrinterService.Client;
using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;
using NLog;
using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Preferences : Form
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ApiClient _apiClient;

        public Preferences()
        {
            _apiClient = new ApiClient();
            InitializeComponent();
            this.groupStore.Text = AppMessages.GroupBasicInfo;
            this.groupPrinter.Text = AppMessages.GroupDataPrinter;
            this.buttonReconnectStores.SetToolTip(AppMessages.ButtonReconnectStores);
            this.buttonPrinterEvents.SetToolTip(AppMessages.ButtonReconnectPrintEvents);
        }

        private async void ButtonReconnectStores_Click(object sender, EventArgs e)
        {
            this.comboStore.Enabled = false;
            await GetAllStoresAsync();
            this.comboStore.Enabled = true;
        }

        private void ButtonSave_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private async Task<List<StoreInfoDTO>> GetAllStoresAsync()
        {
            var stores = new List<StoreInfoDTO>();
            try
            {
                _logger.Info("GetAllStoresAsync():: Listando todos los restaurantes.");
                stores = await _apiClient.GetAllStoresAsync();
                this.comboStore.DataSource = stores;
                this.comboStore.DisplayMember = "StoreName";
                this.comboStore.ValueMember = "BusinessName";
                _logger.Info("GetAllStoresAsync():: Resultado de la petición.", stores.ToJson());
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetAllStoresAsync():: Ha ocurrido un error al obtener el listado de restaurantes.");
                MessageBox.Show(AppMessages.ErrorListStores, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return stores;
        }

        private void GetAllPrinters()
        {
            var printers = new List<PrinterInfo>()
            {
                new PrinterInfo()
            };
            var printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer");
            foreach (var printer in printerQuery.Get())
            {
                var printerInfo = new PrinterInfo
                {
                    Name = printer.GetPropertyValue("Name").ToString(),
                    Status = printer.GetPropertyValue("Status").ToString(),
                    IsDefault = Convert.ToBoolean(printer.GetPropertyValue("Default")),
                    IsNetwork = Convert.ToBoolean(printer.GetPropertyValue("Network")),
                    PortName = printer.GetPropertyValue("PortName").ToString(),
                    DeviceId = printer.GetPropertyValue("DeviceID").ToString(),
                    PrinterState = Convert.ToInt32(printer.GetPropertyValue("PrinterState")),
                    PrinterStatus = Convert.ToInt16(printer.GetPropertyValue("PrinterStatus"))
                };
                printers.Add(printerInfo);
            }
            this.comboPrinters.DataSource = printers;
            this.comboPrinters.DisplayMember = "Name";
            this.comboPrinters.ValueMember = "Name";
        }

        private async void Preferences_Load(object sender, System.EventArgs e)
        {
            await GetAllStoresAsync();
            GetAllPrinters();
        }
    }
}
