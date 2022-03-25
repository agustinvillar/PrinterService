using Menoo.PrinterService.Client;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        private readonly ApiClient _restClient;

        public Main()
        {
            _restClient = new ApiClient();
            InitializeComponent();
        }

        internal void TurnNotification(bool showInTaskbar, bool enableNotificationArea)
        {
            ShowInTaskbar = showInTaskbar;
            notifyClient.Visible = enableNotificationArea;
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string sectorId = ConfigurationManager.AppSettings["sectorId"].ToString();
            if (string.IsNullOrEmpty(sectorId))
            {
                TurnNotification(false, false);
                var preferences = new Preferences();
                WindowState = FormWindowState.Normal;
                preferences.ShowDialog(this);
            }
            else
            {
                string storeName = ConfigurationManager.AppSettings["storeName"].ToString();
                string sectorName = ConfigurationManager.AppSettings["sectorName"].ToString();
                string init = string.Format(AppMessages.SectorInit, sectorName);
                string title = $"{storeName} - {sectorName}";
                this.Text = title;
                this.notifyClient.Text = title;
                this.toolStripStatusLabel.Text = init;
                this.notifyClient.BalloonTipText = init;
                this.notifyClient.BalloonTipIcon = ToolTipIcon.Info;
                this.notifyClient.BalloonTipTitle = AppMessages.TextNotice;
                this.notifyClient.ShowBalloonTip(Constants.NOTICON_DURATION);
            }

        }

        private void NotifyPrinter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            TurnNotification(true, false);
        }

        private void PrinterMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                return;
            }
            TurnNotification(false, true);
        }
    }
}
