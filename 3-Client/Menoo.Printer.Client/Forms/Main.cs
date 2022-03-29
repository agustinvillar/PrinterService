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
using System.Printing;
using System.Threading;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        private IHubProxy _proxy;

        private string _printer;

        private string _storeId;

        private string _sectorId;

        private const int RETRY_CONNECTION_SECONDS = 5;

        private readonly Logger _logger;

        private HubConnection _hubConnection;

        public Main()
        {
            InitializeComponent();
            _logger = LogManager.GetCurrentClassLogger();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        internal void TurnNotification(bool showInTaskbar, bool enableNotificationArea)
        {
            ShowInTaskbar = showInTaskbar;
            notifyClient.Visible = enableNotificationArea;
        }

        private bool CheckTicketIsPrinted()
        {
            bool isPrinted = true;
            var myPrintServer = new PrintServer();
            var printQueues = myPrintServer.GetPrintQueues();
            try
            {
                foreach (PrintQueue queue in printQueues)
                {
                    queue.Refresh();
                    PrintJobInfoCollection pCollection = queue.GetPrintJobInfoCollection();
                    foreach (PrintSystemJobInfo job in pCollection)
                    {
                        isPrinted = SpotTroubleUsingJobAttributes(job);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                //throw;
            }
            return isPrinted;
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
                    _logger.Info($"InitializeSignalRConnection():: Conectado, id de conexión: {_hubConnection.ConnectionId}");
                    _proxy.Invoke("subscribe", _hubConnection.ConnectionId, _sectorId);
                    _proxy.On<string, string, string, int>("recieveTicket", (ticketId, printEvent, ticket, copies) =>
                    {
                        _logger.Info($"Ticket {ticketId} recibido, copias: {copies}.");
                        UpdateDataGridView_OnRecieveTicket(ticketId, printEvent, copies);
                        PrintTicket(ticket, copies);
                        bool isPrinted = CheckTicketIsPrinted();
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
            _printer = ConfigurationManager.AppSettings["printer"].ToString();
            _sectorId = ConfigurationManager.AppSettings["storeId"].ToString();
            _storeId = ConfigurationManager.AppSettings["storeId"].ToString();
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
            printerMessageDTOBindingSource.DataSource = new List<PrinterMessageDTO>();
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

        private void PrintTicket(string image, int copies)
        {
            //var document = new PrintDocument
            //{
            //    PrintController = new StandardPrintController()
            //};
            //document.PrintPage += (base64, args) => PrintPage(image, args);
            //document.PrinterSettings = new PrinterSettings
            //{
            //    PrinterName = ConfigurationManager.AppSettings["printer"],
            //    MaximumPage = copies
            //};
            //document.Print();
            Bitmap bmp;
            var printer = new ESC_POS_USB_NET.Printer.Printer(_printer);
            var imageBytes = Convert.FromBase64String(image);
            using (var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                bmp = new Bitmap(Image.FromStream(memoryStream, true));
            }
            bmp.Save("ticket.png", System.Drawing.Imaging.ImageFormat.Png);
            printer.Image(bmp);
            printer.FullPaperCut();
            for (int i = 1; i <= copies; i++)
            {
                printer.PrintDocument();
            }
            bmp.Dispose();
        }

        //private void PrintPage(object imageBase64, PrintPageEventArgs e)
        //{
        //    Image image;
        //    byte[] imageBytes = Convert.FromBase64String(imageBase64.ToString());
        //    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
        //    {
        //        image = Image.FromStream(ms, true);
        //    }
        //    Point point = new Point();
        //    e.Graphics.DrawImage(image, point);
        //}
        private void PropertiesMenuItem_Click(object sender, EventArgs e)
        {
            var updatePreferences = new UpdatePreferences();
            updatePreferences.ShowDialog(this);
        }

        private bool SpotTroubleUsingJobAttributes(PrintSystemJobInfo job)
        {
            bool isPrinted = false;
            if (((job.JobStatus & PrintJobStatus.Completed) == PrintJobStatus.Completed)
                ||
                ((job.JobStatus & PrintJobStatus.Printed) == PrintJobStatus.Printed))
            {
                //listBox1.Items.Add(
                //    "The job has finished. Have user recheck all output bins and be sure the correct printer is being checked.");
                isPrinted = true;
            }
            if ((job.JobStatus & PrintJobStatus.Error) == PrintJobStatus.Error)
            {
                //listBox1.Items.Add("The job has errored.");
            }
            if ((job.JobStatus & PrintJobStatus.Offline) == PrintJobStatus.Offline)
            {
                //listBox1.Items.Add("The printer is offline. Have user put it online with printer front panel.");
            }
            if ((job.JobStatus & PrintJobStatus.PaperOut) == PrintJobStatus.PaperOut)
            {
                //listBox1.Items.Add("The printer is out of paper of the size required by the job. Have user add paper.");
            }
            if ((job.JobStatus & PrintJobStatus.Printing) == PrintJobStatus.Printing)
            {
                //listBox1.Items.Add("The job is printing now.");
                isPrinted = true;
            }
            if ((job.JobStatus & PrintJobStatus.UserIntervention) == PrintJobStatus.UserIntervention)
            {
                //listBox1.Items.Add("The printer needs human intervention.");
            }
            return isPrinted;
        }

        private void UpdateDataGridView_OnRecieveTicket(string ticketId, string printEvent, int copies)
        {
            printerMessageDTOBindingSource.Add(new PrinterMessageDTO
            {
                Copies = copies,
                Event = printEvent,
                Id = Guid.Parse(ticketId),
                Status = AppMessages.TicketRecieved
            });
            dataGridViewPrintEvents.Update();
            dataGridViewPrintEvents.Refresh();
        }
    }
}
