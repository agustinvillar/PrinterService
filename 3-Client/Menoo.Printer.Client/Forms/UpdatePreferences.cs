using Menoo.PrinterService.Client;
using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using Menoo.PrinterService.Client.Validators;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListBox;

namespace Menoo.Printer.Client
{
    public partial class UpdatePreferences : Form
    {
        private const int PADDING_ERROR_PROVIDER = 5;

        private readonly Logger _logger;

        private readonly ApiClient _apiClient;

        private readonly List<PrintEventsDTO> _prinEvents;

        public UpdatePreferences()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            _prinEvents = new List<PrintEventsDTO>();
            _logger = LogManager.GetCurrentClassLogger();
            string sectorName = ConfigurationManager.AppSettings["sectorName"].ToString();
            this.Text = string.Format(AppMessages.UpdatePreferences, sectorName);
            this.groupPrinter.Text = AppMessages.GroupDataPrinter;
            this.buttonPrinterEvents.SetToolTip(AppMessages.ButtonReconnectPrintEvents);
            this.buttonReconnectPrinters.SetToolTip(AppMessages.ButtonReconnectPrinters);
            errorProvider.SetError(nameTextBox, string.Empty);
            errorProvider.SetIconPadding(nameTextBox, PADDING_ERROR_PROVIDER);
            errorProvider.SetError(printerComboBox, string.Empty);
            errorProvider.SetIconPadding(printerComboBox, PADDING_ERROR_PROVIDER);
            errorProvider.SetError(printEventsListBox, string.Empty);
            errorProvider.SetIconPadding(printEventsListBox, PADDING_ERROR_PROVIDER);
            errorProvider.SetError(copiesNumericUpDown, string.Empty);
            errorProvider.SetIconPadding(copiesNumericUpDown, PADDING_ERROR_PROVIDER);
        }

        private async void ButtonPrinterEvents_Click(object sender, EventArgs e)
        {
            this.printEventsListBox.Enabled = false;
            await GetAllPrintEventsAsync();
            this.printEventsListBox.Enabled = true;
        }

        private void ButtonReconnectPrinters_Click(object sender, EventArgs e)
        {
            this.printerComboBox.Enabled = false;
            GetAllPrinters();
            this.printerComboBox.Enabled = true;
        }

        private async void ButtonSave_Click(object sender, System.EventArgs e)
        {
            var request = (updatePrinterPreferencesRequestBindingSource.Current as UpdatePrinterPreferencesRequest);
            SetPrinterEvents(request, this.printEventsListBox.SelectedItems);
            ValidateForm(request, out bool isValid);
            if (isValid)
            {
                await _apiClient.UpdatePrintPreferencesAsync(request);
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["sectorName"].Value = this.nameTextBox.Text;
                config.AppSettings.Settings["printer"].Value = this.printerComboBox.SelectedValue.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                UpdateMainForm();
                _logger.Info($"ButtonSave_Click():: Sector de impresión : { this.nameTextBox.Text } actualizado correctamente.");
                this.Close();
            }
        }

        private void GetAllPrinters()
        {
            var printers = new List<PrinterInfoDTO>()
            {
                new PrinterInfoDTO
                {
                    Name = AppMessages.Empty
                }
            };
            try
            {
                var printerQuery = new ManagementObjectSearcher("SELECT * from Win32_Printer");
                foreach (var printer in printerQuery.Get())
                {
                    var printerInfo = new PrinterInfoDTO
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
                this.printerComboBox.DataSource = printers;
                this.printerComboBox.DisplayMember = "Name";
                this.printerComboBox.ValueMember = "Name";
                _logger.Info($"GetAllPrinters():: Resultado de la petición. {Environment.NewLine} {printers.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetAllPrinters()::Ha ocurrido un error al obtener el listado de impresoras.");
                MessageBox.Show(AppMessages.ErrorListPrinters, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<List<PrintEventsDTO>> GetAllPrintEventsAsync()
        {
            var printEvents = new List<PrintEventsDTO>();
            try
            {
                _logger.Info("GetAllStoresAsync()::Listando todos los restaurantes.");
                printEvents = await _apiClient.GetAllPrintEventsAsync();
                _prinEvents.AddRange(printEvents);
                this.printEventsListBox.DataSource = printEvents;
                this.printEventsListBox.DisplayMember = "DisplayName";
                this.printEventsListBox.ValueMember = "Id";
                _logger.Info($"GetAllStoresAsync():: Resultado de la petición. {Environment.NewLine} {printEvents.ToJson()}");
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetAllPrintEventsAsync()::Ha ocurrido un error al obtener los eventos de impresión.");
                MessageBox.Show(AppMessages.ErrorListPrintEvents, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return printEvents;
        }

        private void Preferences_FormClosed(object sender, FormClosedEventArgs e)
        {
            var mainForm = (Main)Owner;
            mainForm.TurnNotification(false, true);
        }

        private async void Preferences_Load(object sender, System.EventArgs e)
        {
            GetAllPrinters();
            await GetAllPrintEventsAsync();
            string sectorId = ConfigurationManager.AppSettings["sectorId"].ToString();
            var currentConfiguration = await _apiClient.GetPrinterConfigurationAsync(sectorId);
            updatePrinterPreferencesRequestBindingSource.DataSource = currentConfiguration;
            foreach (var @event in currentConfiguration.PrintEvents)
            {
                var element = this._prinEvents.FirstOrDefault(f => f.Id == @event);
                if (element != null)
                {
                    int index = this.printEventsListBox.Items.IndexOf(element);
                    if (index >= 0)
                    {
                        printEventsListBox.SetSelected(index, true);
                    }
                }
            }
            #region enable controls
            this.nameTextBox.Enabled = true;
            this.printerComboBox.Enabled = true;
            this.buttonReconnectPrinters.Enabled = true;
            this.printEventsListBox.Enabled = true;
            this.buttonPrinterEvents.Enabled = true;
            this.copiesNumericUpDown.Enabled = true;
            this.allowLogoCheckBox.Enabled = true;
            this.allowPrintQRCheckBox.Enabled = true;
            #endregion
        }

        private void SetPrinterEvents(UpdatePrinterPreferencesRequest request, SelectedObjectCollection selectedObjectCollection)
        {
            var values = new List<Guid>();
            foreach (PrintEventsDTO item in selectedObjectCollection)
            {
                values.Add(item.Id);
            }
            request.PrintEvents = values;
        }

        private void UpdateMainForm()
        {
            var mainForm = (Main)Owner;
            string title = $"{ConfigurationManager.AppSettings["storeName"]} - {ConfigurationManager.AppSettings["sectorName"]}";
            string statusMessage = string.Format(AppMessages.UpdatePrintPreferencesOK, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            mainForm.Text = title;
            mainForm.toolStripStatusLabel.Text = statusMessage;
            mainForm.notifyClient.BalloonTipText = statusMessage;
            mainForm.notifyClient.BalloonTipIcon = ToolTipIcon.Info;
            mainForm.notifyClient.BalloonTipTitle = AppMessages.TextNotice;
            mainForm.notifyClient.Text = title;
            mainForm.notifyClient.ShowBalloonTip(Constants.NOTICON_DURATION);
        }

        private void ValidateForm(UpdatePrinterPreferencesRequest request, out bool isValid)
        {
            errorProvider.Clear();
            if (request != null)
            {
                var validator = new UpdatePreferencesPrinterRequestValidator();
                var result = validator.Validate(request);
                isValid = result.IsValid;
                if (!isValid)
                {
                    foreach (var error in result.Errors)
                    {
                        switch (error.PropertyName)
                        {
                            case "Name":
                                errorProvider.SetError(nameTextBox, error.ErrorMessage);
                                break;
                            case "Printer":
                                errorProvider.SetError(printerComboBox, error.ErrorMessage);
                                break;
                            case "PrintEvents":
                                errorProvider.SetError(printEventsListBox, error.ErrorMessage);
                                break;
                            case "Copies":
                                errorProvider.SetError(copiesNumericUpDown, error.ErrorMessage);
                                break;
                        }
                        MessageBox.Show(error.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            else
            {
                isValid = false;
                MessageBox.Show(AppMessages.FormEmpty, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCloseUpdatePreferences_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
