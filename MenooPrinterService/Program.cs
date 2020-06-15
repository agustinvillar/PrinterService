using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenooPrinterService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var procesos = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (procesos.Count() > 1)
            {
                MessageBox.Show("La aplicación ya se encuentra abierta.", "Menoo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            bool runWithSettings = args != null && args.Any(a => a.Contains("settings"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Splash(runWithSettings));
        }
    }
}
