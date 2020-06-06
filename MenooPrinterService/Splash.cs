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
    public partial class Splash : Form
    {
        private Timer splashTimer;
        private Timer progressBarTimer;
        private bool runWithoutSettings;
        public Splash(bool runWithoutSettings)
        {
            this.runWithoutSettings = runWithoutSettings;
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MinimizeBox = false;
            this.progressBar.Value = 1;
            this.progressBar.Step = 1;
            this.progressBar.Minimum = 1;
            this.progressBar.Maximum = 1000;
        }

        private void Splash_Shown(object sender, EventArgs e)
        {
            splashTimer = new Timer();
            splashTimer.Interval = 1000;
            splashTimer.Start();
            splashTimer.Tick += tmr_Tick;

            progressBarTimer = new Timer();
            progressBarTimer.Interval = 10;
            progressBarTimer.Start();
            progressBarTimer.Tick += ProgressBar_Tick;

        }
        public void tmr_Tick(object sender, EventArgs e)
        {
            splashTimer.Stop();
            MenooPrinter menooPrinter = new MenooPrinter(this, runWithoutSettings);
            menooPrinter.Show();
            this.Hide();
        }
        public void ProgressBar_Tick(object sender, EventArgs e)
        {
            try
            {
                this.progressBar.Value += 40;
            }
            catch (Exception ex)
            {
                this.progressBar.Value = this.progressBar.Maximum;
            }
        }
    }
}
