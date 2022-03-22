using System;
using System.Configuration;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void DisableNotification(bool showInTaskbar, bool enableNotificationArea)
        {
            ShowInTaskbar = showInTaskbar;
            notifyClient.Visible = enableNotificationArea;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["sectorId"]))
            {
                DisableNotification(false, false);
                var preferences = new Preferences();
                WindowState = FormWindowState.Normal;
                preferences.ShowDialog();
            }
        }

        private void NotifyPrinter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            DisableNotification(true, false);
        }

        private void PrinterMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                return;
            }
            DisableNotification(false, true);
        }
    }
}
