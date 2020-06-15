using Dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenooPrinterService
{
    public partial class Confirmar : Form
    {
        private string[] Printers;
        private readonly bool clean;

        public Confirmar(bool clean)
        {
            this.clean = clean;
            InitializeComponent();
            Init();
        }
        private async void Init()
        {
            this.DdlImpresoras.DataSource = await LoadPrintersAsync();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.LoadInfo();
        }

        private async Task LoadStores()
        {
            DdlStores.Enabled = false;
            DdlStores.DataSource = await Firebase.GetStores();
            DdlStores.ValueMember = "Id";
            DdlStores.DisplayMember = "Name";
            DdlStores.Enabled = true;
        }

        private Task<string[]> LoadPrintersAsync()
        {
            return Task.Run(() =>
            {
                var printers = new List<string>();
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    printers.Add(printer);
                Printers = printers.ToArray();
                return Printers;
            });
        }

        private async void LoadInfo()
        {
            this.DdlImpresoras.SelectedIndex = this.DdlImpresoras.FindStringExact(ConfigurationManager.AppSettings["PrinterName"]);
            await this.LoadStores();
            this.DdlStores.SelectedValue = ConfigurationManager.AppSettings["StoreID"];
        }

        private Task ConfirmRunAsync(string printer, string storeId)
        {
            return Task.Run(() =>
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                string printerName = config.AppSettings.Settings["PrinterName"].Value;
                config.AppSettings.Settings["PrinterName"].Value = printer;

                string storeID = config.AppSettings.Settings["StoreID"].Value;
                config.AppSettings.Settings["StoreID"].Value = storeId;

                config.Save(ConfigurationSaveMode.Modified);

                Firebase.RunAsync(this.clean);
            });
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                BtnGuardar.Enabled = false;
                var dialog = MessageBox.Show("¿Desea continuar?", "OK", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    string printer = this.DdlImpresoras.SelectedValue.ToString();
                    string storeId = this.DdlStores.SelectedValue.ToString();
                    Store store = this.DdlStores.SelectedItem as Store;
                    string storeName = store == null ? string.Empty : store.Name;

                    ConfirmRunAsync(printer, storeId);

                    MessageBox.Show($"Se configuró el resto {storeName} con la impresora {printer}",
                        "Menoo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    this.LoadInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
