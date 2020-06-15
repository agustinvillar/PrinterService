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
        public MenooPrinter(Form splash, bool runWithSettings)
        {
            InitializeComponent();
            Init(runWithSettings);
            this.splash = splash;
        }
        private void Init(bool runWithSettings)
        {
            try
            {
                if (runWithSettings)
                {
                    Confirmar confirmar = new Confirmar();
                    confirmar.Show();
                }
                else
                {
                    Firebase.RunAsync();
                }
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
