using System;
using System.Windows.Forms;

namespace Menoo.Printer.Client
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void PrinterMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                return;
            }
            ShowInTaskbar = false;
            notifyClient.Visible = true;
        }

        private void NotifyPrinter_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyClient.Visible = false;
        }
    }
}
