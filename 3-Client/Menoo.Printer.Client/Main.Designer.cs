using System.Drawing;

namespace Menoo.Printer.Client
{
    partial class Main
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.statusArea = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.notifyClient = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // statusArea
            // 
            this.statusArea.Location = new System.Drawing.Point(0, 245);
            this.statusArea.Name = "statusArea";
            this.statusArea.Size = new System.Drawing.Size(672, 22);
            this.statusArea.TabIndex = 0;
            this.statusArea.Text = "statusArea";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(672, 245);
            this.panel1.TabIndex = 1;
            // 
            // notifyClient
            // 
            this.notifyClient.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyClient.Icon")));
            this.notifyClient.Text = "Sector de impresión";
            this.notifyClient.Visible = true;
            this.notifyClient.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyPrinter_MouseDoubleClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(672, 267);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusArea);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Printer";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.SizeChanged += new System.EventHandler(this.PrinterMain_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.StatusStrip statusArea;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NotifyIcon notifyClient;
    }
}

