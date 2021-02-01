using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menoo.PrinterService.App
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            this.TxtMenoo.Text = $"Sistema de Impresion {DateTime.Now.Year} ©";
            this.TxtVersion.Text = $"Version {ProductVersion}";
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
    }
}
