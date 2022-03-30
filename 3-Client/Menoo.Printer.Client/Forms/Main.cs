using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using Microsoft.AspNet.SignalR.Client;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        private const int RETRY_CONNECTION_SECONDS = 5;

        private readonly Logger _logger;

        private HubConnection _hubConnection;

        private string _printer;

        private IHubProxy _proxy;

        private string _sectorId;

        private string _storeId;

        public Main()
        {
            InitializeComponent();
            _logger = LogManager.GetCurrentClassLogger();
            dataGridViewPrintEvents.RowHeadersVisible = false;
            Control.CheckForIllegalCrossThreadCalls = false;
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
                _logger.Warn("CloseToolStripMenuItem_Click():: Se ha apagado el sector de impresión.");
                this.Close();
            }
        }

        private async Task InitializeSignalRConnectionAsync()
        {
            string baseUrl = ConfigurationManager.AppSettings["signalR"];
            _hubConnection = new HubConnection(baseUrl);
            _hubConnection.Closed += OnDisconnected;
            _proxy = _hubConnection.CreateHubProxy("PrintHub");
            try
            {
                _logger.Info("InitializeSignalRConnectionAsync():: Conectando con el servidor de impresión.");
                await _hubConnection.Start();
                if (_hubConnection.State == ConnectionState.Connected)
                {
                    _logger.Info($"InitializeSignalRConnection():: Conectado, id de conexión: {_hubConnection.ConnectionId}");
                    this.toolStripStatusLabel.Text = string.Format(AppMessages.SignalRConnectionEntablished, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    await _proxy.Invoke("subscribe", _hubConnection.ConnectionId, _sectorId);
                    _proxy.On<string, string, string, int>("recieveTicket", (ticketId, printEvent, ticket, copies) =>
                    {
                        _logger.Info($"Ticket {ticketId} recibido, copias: {copies}.");
                        UpdateDataGridView_OnRecieveTicket(ticketId, printEvent, copies);
                        PrintTicket(ticket, copies);
                        UpdateDataGridView_OnPrintedTicket(ticketId);
                        _proxy.Invoke("markAsPrinted", ticketId, _storeId, printEvent).GetAwaiter().GetResult();
                        _logger.Info($"Ticket {ticketId} impreso.");
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"InitializeSignalRConnectionAsync():: {ex.Message}.");
                this.toolStripStatusLabel.Text = AppMessages.SignalRConnectionNotEntablished;
                await InitializeSignalRConnectionAsync();
            }
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            _sectorId = ConfigurationManager.AppSettings["sectorId"].ToString();
            if (string.IsNullOrEmpty(_sectorId))
            {
                WindowState = FormWindowState.Normal;
                TurnNotification(false, false);
                var preferences = new Preferences();
                preferences.ShowDialog(this);
            }
            _printer = ConfigurationManager.AppSettings["printer"].ToString();
            _storeId = ConfigurationManager.AppSettings["storeId"].ToString();
            string storeName = ConfigurationManager.AppSettings["storeName"].ToString();
            string sectorName = ConfigurationManager.AppSettings["sectorName"].ToString();
            string init = string.Format(AppMessages.SectorInit, sectorName);
            string title = $"{storeName} - {sectorName}";
            this.Text = title;
            this.toolStripStatusLabel.Text = init;
            this.notifyClient.Text = title;
            this.notifyClient.BalloonTipText = init;
            this.notifyClient.BalloonTipIcon = ToolTipIcon.Info;
            this.notifyClient.BalloonTipTitle = AppMessages.TextNotice;
            this.notifyClient.ShowBalloonTip(Constants.NOTICON_DURATION);
            printerMessageDTOBindingSource.DataSource = new List<PrinterMessageDTO>();
            await InitializeSignalRConnectionAsync();
        }

        private void NotifyPrinter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            TurnNotification(true, false);
        }

        private async void OnDisconnected()
        {
            Thread.Sleep(TimeSpan.FromSeconds(RETRY_CONNECTION_SECONDS));
            await InitializeSignalRConnectionAsync();
        }

        private void PrinterMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                return;
            }
            TurnNotification(false, true);
        }

        private void PrintTicket(string image, int copies)
        {
            Bitmap bmp;
            var printer = new ESC_POS_USB_NET.Printer.Printer(_printer);
            var imageBytes = Convert.FromBase64String(image);
            using (var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                bmp = new Bitmap(Image.FromStream(memoryStream, true));
            }
            printer.Image(bmp);
            printer.FullPaperCut();
            for (int i = 1; i <= copies; i++)
            {
                printer.PrintDocument();
            }
            bmp.Dispose();
        }

        private void PropertiesMenuItem_Click(object sender, EventArgs e)
        {
            var updatePreferences = new UpdatePreferences();
            updatePreferences.ShowDialog(this);
        }

        private void UpdateDataGridView_OnPrintedTicket(string ticketId)
        {
            var id = Guid.Parse(ticketId);
            var ticketHistory = printerMessageDTOBindingSource.DataSource as List<PrinterMessageDTO>;
            var item = ticketHistory.FirstOrDefault(f => f.Id == id);
            item.Status = AppMessages.TicketPrinted;
            dataGridViewPrintEvents.Update();
            dataGridViewPrintEvents.Refresh();
            this.toolStripStatusLabel.Text = string.Format(AppMessages.LastTicketPrinted, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        private void UpdateDataGridView_OnRecieveTicket(string ticketId, string printEvent, int copies)
        {
            printerMessageDTOBindingSource.Add(new PrinterMessageDTO
            {
                Copies = copies,
                Event = printEvent,
                Id = Guid.Parse(ticketId),
                Status = AppMessages.TicketRecieved,
                RecievedAt = DateTime.Now
            });
            dataGridViewPrintEvents.Update();
            dataGridViewPrintEvents.Refresh();
        }
    }
}
