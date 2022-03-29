using Menoo.PrinterService.Client;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using Microsoft.AspNet.SignalR.Client;
using NLog;
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        private const int RETRY_CONNECTION_SECONDS = 5;

        private readonly Logger _logger;

        private readonly ApiClient _restClient;

        private HubConnection _hubConnection;

        private IHubProxy _proxy;

        public Main()
        {
            InitializeComponent();
            _restClient = new ApiClient();
            _logger = LogManager.GetCurrentClassLogger();
        }

        internal void TurnNotification(bool showInTaskbar, bool enableNotificationArea)
        {
            ShowInTaskbar = showInTaskbar;
            notifyClient.Visible = enableNotificationArea;
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show(AppMessages.WarningCloseClient, AppMessages.TextNotice, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (response == DialogResult.OK)
            {
                _hubConnection.Stop();
                _logger.Warn("CloseToolStripMenuItem_Click():: Se ha apagado el sector de impresión.");
                this.Close();
            }
        }

        private void InitializeSignalRConnection()
        {
            string baseUrl = ConfigurationManager.AppSettings["signalR"];
            _hubConnection = new HubConnection(baseUrl);
            _hubConnection.Closed += OnDisconnected;
            _proxy = _hubConnection.CreateHubProxy("PrintHub");
            _hubConnection.Start().ContinueWith(connectionTask =>
            {
                if (!connectionTask.IsFaulted)
                {
                    _logger.Info($"InitializeSignalRConnection():: Conectado, id de conexión: {connectionTask.Id}");
                    string storeId = ConfigurationManager.AppSettings["storeId"];
                    string sectorId = ConfigurationManager.AppSettings["sectorId"];
                    _proxy.Invoke("subscribe", storeId, sectorId);
                    _proxy.On<string, int>("recieveTicket", (ticket, copies) =>
                    {
                        //TODO: Imprimir ticket
                        _logger.Info($"Ticket de impresión recibido, copias: {copies}.{Environment.NewLine}{ticket}")
                    });
                }
                else
                {
                    InitializeSignalRConnection();
                }
            }).Wait();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string sectorId = ConfigurationManager.AppSettings["sectorId"].ToString();
            if (string.IsNullOrEmpty(sectorId))
            {
                WindowState = FormWindowState.Normal;
                TurnNotification(false, false);
                var preferences = new Preferences();
                preferences.ShowDialog(this);
            }
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
            InitializeSignalRConnection();
        }

        private void NotifyPrinter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            TurnNotification(true, false);
        }

        private void OnDisconnected()
        {
            Thread.Sleep(TimeSpan.FromSeconds(RETRY_CONNECTION_SECONDS));
            bool isConnected = false;

            if (!isConnected)
            {
                InitializeSignalRConnection();
            }
        }

        private void PrinterMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                return;
            }
            TurnNotification(false, true);
        }

        private void PropertiesMenuItem_Click(object sender, EventArgs e)
        {
            var updatePreferences = new UpdatePreferences();
            updatePreferences.ShowDialog(this);
        }
    }
}
