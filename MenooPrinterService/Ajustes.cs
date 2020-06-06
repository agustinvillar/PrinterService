﻿using Dominio;
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
    public partial class Ajustes : Form
    {
        private string[] Printers;

        public Ajustes()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            var printers = new List<string>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                printers.Add(printer);
            Printers = printers.ToArray();

            this.DdlImpresoras.DataSource = Printers;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.LoadInfo();
        }

        private void LoadInfo()
        {
            this.DdlImpresoras.SelectedIndex = this.DdlImpresoras.FindStringExact(ConfigurationManager.AppSettings["PrinterName"]);
            this.TxtRestoId.Text = ConfigurationManager.AppSettings["StoreID"];
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = MessageBox.Show("¿Desea guardar los cambios?", "Guardar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

                    string printerName = config.AppSettings.Settings["PrinterName"].Value;
                    config.AppSettings.Settings["PrinterName"].Value = this.DdlImpresoras.SelectedValue.ToString();

                    string storeID = config.AppSettings.Settings["StoreID"].Value;
                    config.AppSettings.Settings["StoreID"].Value = this.TxtRestoId.Text;

                    config.Save(ConfigurationSaveMode.Modified);

                    Firebase.RefreshListener(this.TxtRestoId.Text);

                    MessageBox.Show("Guardado OK", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
