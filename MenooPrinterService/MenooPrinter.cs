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
        public MenooPrinter()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Firebase.Instancia.GetOrders();
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var infoWindow = new Info();
            infoWindow.Visible = true;
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NotifyIcon.Dispose();
            this.Close();
        }

        private void AjustesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ajustesWindow = new Ajustes();
            ajustesWindow.Visible = true;
        }

        private void MenooPrinter_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }
    }
}
