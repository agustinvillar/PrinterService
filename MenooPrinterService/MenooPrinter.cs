﻿using Menoo.PrinterService.Business;
using System;
using System.Windows.Forms;

namespace Menoo.PrinterService.App
{
    public partial class MenooPrinter : Form
    {
        private readonly Form splash;
        public MenooPrinter(Form splash)
        {
            InitializeComponent();
            Init();
            this.splash = splash;
        }
        private void Init()
        {
            try
            {
                Firebase.RunAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var infoWindow = new Info();
            infoWindow.Visible = true;
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("¿Desea salir?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                this.Close();
                this.splash.Close();
                this.NotifyIcon.Dispose();
            }
        }

        private void AjustesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void MenooPrinter_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInfo();
        }
        private void ShowInfo()
        {
            var window = new Info();
            window.Show();
        }
    }
}
