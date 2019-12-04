using Dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenooPrinterService
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
        private async void Init()
        {
            await Firebase.Instancia.RunListenOrders();
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
            this.ShowAjustes();
        }

        private void MenooPrinter_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowAjustes();
        }
        private void ShowAjustes()
        {
            var ajustesWindow = new Ajustes();
            ajustesWindow.Visible = true;
        }
    }
}
