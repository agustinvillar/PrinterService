using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;

namespace Menoo.Printer.Client
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.labelPrinter = new System.Windows.Forms.Label();
            this.labelSectorName = new System.Windows.Forms.Label();
            this.textSectorName = new System.Windows.Forms.TextBox();
            this.comboPrinters = new System.Windows.Forms.ComboBox();
            this.groupStore = new System.Windows.Forms.GroupBox();
            this.buttonReconnectStores = new System.Windows.Forms.Button();
            this.comboStore = new System.Windows.Forms.ComboBox();
            this.labelStore = new System.Windows.Forms.Label();
            this.groupPrinter = new System.Windows.Forms.GroupBox();
            this.labelPrintLogo = new System.Windows.Forms.Label();
            this.checkBoxPrintLogo = new System.Windows.Forms.CheckBox();
            this.comboPrintEvents = new System.Windows.Forms.ComboBox();
            this.labelPrintEvents = new System.Windows.Forms.Label();
            this.checkBoxPrintQR = new System.Windows.Forms.CheckBox();
            this.labelCopies = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.txtCopies = new System.Windows.Forms.NumericUpDown();
            this.buttonPrinterEvents = new System.Windows.Forms.Button();
            this.groupStore.SuspendLayout();
            this.groupPrinter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPrinter
            // 
            this.labelPrinter.AutoSize = true;
            this.labelPrinter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrinter.Location = new System.Drawing.Point(6, 28);
            this.labelPrinter.Name = "labelPrinter";
            this.labelPrinter.Size = new System.Drawing.Size(56, 13);
            this.labelPrinter.TabIndex = 0;
            this.labelPrinter.Text = "Impresora:";
            // 
            // labelSectorName
            // 
            this.labelSectorName.AutoSize = true;
            this.labelSectorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSectorName.Location = new System.Drawing.Point(6, 75);
            this.labelSectorName.Name = "labelSectorName";
            this.labelSectorName.Size = new System.Drawing.Size(99, 13);
            this.labelSectorName.TabIndex = 1;
            this.labelSectorName.Text = "Nombre del sector: ";
            // 
            // textSectorName
            // 
            this.textSectorName.AcceptsTab = true;
            this.textSectorName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSectorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textSectorName.Location = new System.Drawing.Point(121, 75);
            this.textSectorName.Name = "textSectorName";
            this.textSectorName.Size = new System.Drawing.Size(184, 20);
            this.textSectorName.TabIndex = 2;
            // 
            // comboPrinters
            // 
            this.comboPrinters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPrinters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboPrinters.FormattingEnabled = true;
            this.comboPrinters.Location = new System.Drawing.Point(121, 28);
            this.comboPrinters.Name = "comboPrinters";
            this.comboPrinters.Size = new System.Drawing.Size(184, 21);
            this.comboPrinters.TabIndex = 3;
            // 
            // groupStore
            // 
            this.groupStore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupStore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupStore.CausesValidation = false;
            this.groupStore.Controls.Add(this.buttonReconnectStores);
            this.groupStore.Controls.Add(this.comboStore);
            this.groupStore.Controls.Add(this.labelStore);
            this.groupStore.Controls.Add(this.labelSectorName);
            this.groupStore.Controls.Add(this.textSectorName);
            this.groupStore.Location = new System.Drawing.Point(12, 12);
            this.groupStore.Name = "groupStore";
            this.groupStore.Size = new System.Drawing.Size(372, 116);
            this.groupStore.TabIndex = 4;
            this.groupStore.TabStop = false;
            // 
            // buttonReconnectStores
            // 
            this.buttonReconnectStores.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReconnectStores.Image = ((System.Drawing.Image)(resources.GetObject("buttonReconnectStores.Image")));
            this.buttonReconnectStores.Location = new System.Drawing.Point(311, 30);
            this.buttonReconnectStores.Name = "buttonReconnectStores";
            this.buttonReconnectStores.Size = new System.Drawing.Size(24, 22);
            this.buttonReconnectStores.TabIndex = 5;
            this.buttonReconnectStores.UseVisualStyleBackColor = true;
            this.buttonReconnectStores.Click += new System.EventHandler(this.ButtonReconnectStores_Click);
            // 
            // comboStore
            // 
            this.comboStore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStore.FormattingEnabled = true;
            this.comboStore.Location = new System.Drawing.Point(121, 30);
            this.comboStore.Name = "comboStore";
            this.comboStore.Size = new System.Drawing.Size(184, 21);
            this.comboStore.TabIndex = 4;
            // 
            // labelStore
            // 
            this.labelStore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStore.AutoSize = true;
            this.labelStore.CausesValidation = false;
            this.labelStore.Location = new System.Drawing.Point(6, 30);
            this.labelStore.Name = "labelStore";
            this.labelStore.Size = new System.Drawing.Size(68, 13);
            this.labelStore.TabIndex = 3;
            this.labelStore.Text = "Restaurante:";
            // 
            // groupPrinter
            // 
            this.groupPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPrinter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupPrinter.Controls.Add(this.buttonPrinterEvents);
            this.groupPrinter.Controls.Add(this.txtCopies);
            this.groupPrinter.Controls.Add(this.labelPrintLogo);
            this.groupPrinter.Controls.Add(this.checkBoxPrintLogo);
            this.groupPrinter.Controls.Add(this.comboPrintEvents);
            this.groupPrinter.Controls.Add(this.labelPrintEvents);
            this.groupPrinter.Controls.Add(this.checkBoxPrintQR);
            this.groupPrinter.Controls.Add(this.labelCopies);
            this.groupPrinter.Controls.Add(this.labelPrinter);
            this.groupPrinter.Controls.Add(this.comboPrinters);
            this.groupPrinter.Location = new System.Drawing.Point(12, 147);
            this.groupPrinter.Name = "groupPrinter";
            this.groupPrinter.Size = new System.Drawing.Size(372, 212);
            this.groupPrinter.TabIndex = 5;
            this.groupPrinter.TabStop = false;
            // 
            // labelPrintLogo
            // 
            this.labelPrintLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPrintLogo.AutoSize = true;
            this.labelPrintLogo.Location = new System.Drawing.Point(6, 172);
            this.labelPrintLogo.Name = "labelPrintLogo";
            this.labelPrintLogo.Size = new System.Drawing.Size(68, 13);
            this.labelPrintLogo.TabIndex = 12;
            this.labelPrintLogo.Text = "Imprimir logo:";
            // 
            // checkBoxPrintLogo
            // 
            this.checkBoxPrintLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPrintLogo.AutoSize = true;
            this.checkBoxPrintLogo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxPrintLogo.Location = new System.Drawing.Point(121, 172);
            this.checkBoxPrintLogo.Name = "checkBoxPrintLogo";
            this.checkBoxPrintLogo.Size = new System.Drawing.Size(15, 14);
            this.checkBoxPrintLogo.TabIndex = 11;
            this.checkBoxPrintLogo.UseVisualStyleBackColor = true;
            // 
            // comboPrintEvents
            // 
            this.comboPrintEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPrintEvents.FormattingEnabled = true;
            this.comboPrintEvents.Location = new System.Drawing.Point(121, 124);
            this.comboPrintEvents.Name = "comboPrintEvents";
            this.comboPrintEvents.Size = new System.Drawing.Size(184, 21);
            this.comboPrintEvents.TabIndex = 10;
            // 
            // labelPrintEvents
            // 
            this.labelPrintEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPrintEvents.AutoSize = true;
            this.labelPrintEvents.Location = new System.Drawing.Point(6, 124);
            this.labelPrintEvents.Name = "labelPrintEvents";
            this.labelPrintEvents.Size = new System.Drawing.Size(91, 13);
            this.labelPrintEvents.TabIndex = 9;
            this.labelPrintEvents.Text = "Tickets a imprimir:";
            // 
            // checkBoxPrintQR
            // 
            this.checkBoxPrintQR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPrintQR.AutoSize = true;
            this.checkBoxPrintQR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxPrintQR.Location = new System.Drawing.Point(196, 72);
            this.checkBoxPrintQR.Name = "checkBoxPrintQR";
            this.checkBoxPrintQR.Size = new System.Drawing.Size(80, 17);
            this.checkBoxPrintQR.TabIndex = 8;
            this.checkBoxPrintQR.Text = "Imprimir QR";
            this.checkBoxPrintQR.UseMnemonic = false;
            this.checkBoxPrintQR.UseVisualStyleBackColor = true;
            // 
            // labelCopies
            // 
            this.labelCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCopies.AutoSize = true;
            this.labelCopies.CausesValidation = false;
            this.labelCopies.Location = new System.Drawing.Point(6, 73);
            this.labelCopies.Name = "labelCopies";
            this.labelCopies.Size = new System.Drawing.Size(45, 13);
            this.labelCopies.TabIndex = 4;
            this.labelCopies.Text = "Copias: ";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(279, 383);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(105, 29);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Guardar";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // txtCopies
            // 
            this.txtCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCopies.Location = new System.Drawing.Point(121, 72);
            this.txtCopies.Name = "txtCopies";
            this.txtCopies.Size = new System.Drawing.Size(38, 20);
            this.txtCopies.TabIndex = 13;
            this.txtCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonPrinterEvents
            // 
            this.buttonPrinterEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrinterEvents.Image = ((System.Drawing.Image)(resources.GetObject("buttonPrinterEvents.Image")));
            this.buttonPrinterEvents.Location = new System.Drawing.Point(311, 124);
            this.buttonPrinterEvents.Name = "buttonPrinterEvents";
            this.buttonPrinterEvents.Size = new System.Drawing.Size(24, 22);
            this.buttonPrinterEvents.TabIndex = 6;
            this.buttonPrinterEvents.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(402, 442);
            this.ControlBox = false;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupPrinter);
            this.Controls.Add(this.groupStore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración sector";
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.groupStore.ResumeLayout(false);
            this.groupStore.PerformLayout();
            this.groupPrinter.ResumeLayout(false);
            this.groupPrinter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCopies)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelPrinter;
        private System.Windows.Forms.Label labelSectorName;
        private System.Windows.Forms.TextBox textSectorName;
        private System.Windows.Forms.ComboBox comboPrinters;
        private System.Windows.Forms.GroupBox groupStore;
        private System.Windows.Forms.ComboBox comboStore;
        private System.Windows.Forms.Label labelStore;
        private System.Windows.Forms.GroupBox groupPrinter;
        private System.Windows.Forms.Label labelCopies;
        private System.Windows.Forms.CheckBox checkBoxPrintQR;
        private System.Windows.Forms.ComboBox comboPrintEvents;
        private System.Windows.Forms.Label labelPrintEvents;
        private System.Windows.Forms.Label labelPrintLogo;
        private System.Windows.Forms.CheckBox checkBoxPrintLogo;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonReconnectStores;
        private System.Windows.Forms.NumericUpDown txtCopies;
        private System.Windows.Forms.Button buttonPrinterEvents;
    }
}