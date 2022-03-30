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
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.dataGridViewPrintEvents = new System.Windows.Forms.DataGridView();
            this.printerMessageDTOBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyClient = new System.Windows.Forms.NotifyIcon(this.components);
            this.eventDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.copiesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecievedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusArea.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrintEvents)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.printerMessageDTOBindingSource)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusArea
            // 
            this.statusArea.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusArea.Location = new System.Drawing.Point(0, 245);
            this.statusArea.Name = "statusArea";
            this.statusArea.Size = new System.Drawing.Size(443, 22);
            this.statusArea.TabIndex = 0;
            this.statusArea.Text = "statusArea";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.dataGridViewPrintEvents);
            this.mainPanel.Controls.Add(this.menuStrip);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(443, 245);
            this.mainPanel.TabIndex = 1;
            // 
            // dataGridViewPrintEvents
            // 
            this.dataGridViewPrintEvents.AllowUserToAddRows = false;
            this.dataGridViewPrintEvents.AllowUserToDeleteRows = false;
            this.dataGridViewPrintEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewPrintEvents.AutoGenerateColumns = false;
            this.dataGridViewPrintEvents.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewPrintEvents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPrintEvents.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.eventDataGridViewTextBoxColumn,
            this.copiesDataGridViewTextBoxColumn,
            this.Status,
            this.RecievedAt});
            this.dataGridViewPrintEvents.DataSource = this.printerMessageDTOBindingSource;
            this.dataGridViewPrintEvents.Location = new System.Drawing.Point(0, 30);
            this.dataGridViewPrintEvents.MultiSelect = false;
            this.dataGridViewPrintEvents.Name = "dataGridViewPrintEvents";
            this.dataGridViewPrintEvents.ReadOnly = true;
            this.dataGridViewPrintEvents.Size = new System.Drawing.Size(443, 215);
            this.dataGridViewPrintEvents.TabIndex = 1;
            // 
            // printerMessageDTOBindingSource
            // 
            this.printerMessageDTOBindingSource.DataSource = typeof(Menoo.PrinterService.Client.DTOs.PrinterMessageDTO);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem});
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(443, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.propertiesToolStripMenuItem,
            this.toolStripSeparator,
            this.closeToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.configurationToolStripMenuItem.Text = "Impresora";
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("propertiesToolStripMenuItem.Image")));
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.propertiesToolStripMenuItem.Text = "Propiedades";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.PropertiesMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(177, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.closeToolStripMenuItem.Text = "Cerrar";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // notifyClient
            // 
            this.notifyClient.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyClient.Icon")));
            this.notifyClient.Text = "Test";
            this.notifyClient.Visible = true;
            this.notifyClient.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyPrinter_MouseDoubleClick);
            // 
            // eventDataGridViewTextBoxColumn
            // 
            this.eventDataGridViewTextBoxColumn.DataPropertyName = "Event";
            this.eventDataGridViewTextBoxColumn.HeaderText = "Tipo de ticket";
            this.eventDataGridViewTextBoxColumn.Name = "eventDataGridViewTextBoxColumn";
            this.eventDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // copiesDataGridViewTextBoxColumn
            // 
            this.copiesDataGridViewTextBoxColumn.DataPropertyName = "Copies";
            this.copiesDataGridViewTextBoxColumn.HeaderText = "Copias";
            this.copiesDataGridViewTextBoxColumn.Name = "copiesDataGridViewTextBoxColumn";
            this.copiesDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "Estado";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // RecievedAt
            // 
            this.RecievedAt.DataPropertyName = "RecievedAt";
            this.RecievedAt.FillWeight = 20F;
            this.RecievedAt.HeaderText = "Hora recepción";
            this.RecievedAt.Name = "RecievedAt";
            this.RecievedAt.ReadOnly = true;
            this.RecievedAt.Width = 150;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(443, 267);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.statusArea);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Printer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.SizeChanged += new System.EventHandler(this.PrinterMain_SizeChanged);
            this.statusArea.ResumeLayout(false);
            this.statusArea.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPrintEvents)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.printerMessageDTOBindingSource)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        internal System.Windows.Forms.StatusStrip statusArea;
        internal System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        internal System.Windows.Forms.NotifyIcon notifyClient;
        internal System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.DataGridView dataGridViewPrintEvents;
        private System.Windows.Forms.BindingSource printerMessageDTOBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn eventDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn copiesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecievedAt;
    }
}

