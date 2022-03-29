using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;

namespace Menoo.Printer.Client
{
    partial class UpdatePreferences
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label printerLabel;
            System.Windows.Forms.Label printEventsLabel;
            System.Windows.Forms.Label copiesLabel;
            System.Windows.Forms.Label allowPrintQRLabel;
            System.Windows.Forms.Label allowLogoLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdatePreferences));
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonPrinterEvents = new System.Windows.Forms.Button();
            this.buttonReconnectPrinters = new System.Windows.Forms.Button();
            this.groupPrinter = new System.Windows.Forms.GroupBox();
            this.labelEventsSelected = new System.Windows.Forms.Label();
            this.allowLogoCheckBox = new System.Windows.Forms.CheckBox();
            this.allowPrintQRCheckBox = new System.Windows.Forms.CheckBox();
            this.copiesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.printEventsListBox = new System.Windows.Forms.ListBox();
            this.printerComboBox = new System.Windows.Forms.ComboBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.buttonCloseUpdatePreferences = new System.Windows.Forms.Button();
            this.updatePrinterPreferencesRequestBindingSource = new System.Windows.Forms.BindingSource(this.components);
            nameLabel = new System.Windows.Forms.Label();
            printerLabel = new System.Windows.Forms.Label();
            printEventsLabel = new System.Windows.Forms.Label();
            copiesLabel = new System.Windows.Forms.Label();
            allowPrintQRLabel = new System.Windows.Forms.Label();
            allowLogoLabel = new System.Windows.Forms.Label();
            this.groupPrinter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copiesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updatePrinterPreferencesRequestBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new System.Drawing.Point(18, 24);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size(96, 13);
            nameLabel.TabIndex = 6;
            nameLabel.Text = "Nombre del sector:";
            // 
            // printerLabel
            // 
            printerLabel.AutoSize = true;
            printerLabel.Location = new System.Drawing.Point(6, 31);
            printerLabel.Name = "printerLabel";
            printerLabel.Size = new System.Drawing.Size(40, 13);
            printerLabel.TabIndex = 15;
            printerLabel.Text = "Printer:";
            // 
            // printEventsLabel
            // 
            printEventsLabel.AutoSize = true;
            printEventsLabel.Location = new System.Drawing.Point(6, 74);
            printEventsLabel.Name = "printEventsLabel";
            printEventsLabel.Size = new System.Drawing.Size(91, 13);
            printEventsLabel.TabIndex = 16;
            printEventsLabel.Text = "Tickets a imprimir:";
            // 
            // copiesLabel
            // 
            copiesLabel.AutoSize = true;
            copiesLabel.Location = new System.Drawing.Point(6, 277);
            copiesLabel.Name = "copiesLabel";
            copiesLabel.Size = new System.Drawing.Size(42, 13);
            copiesLabel.TabIndex = 17;
            copiesLabel.Text = "Copias:";
            // 
            // allowPrintQRLabel
            // 
            allowPrintQRLabel.AutoSize = true;
            allowPrintQRLabel.Location = new System.Drawing.Point(6, 326);
            allowPrintQRLabel.Name = "allowPrintQRLabel";
            allowPrintQRLabel.Size = new System.Drawing.Size(114, 13);
            allowPrintQRLabel.TabIndex = 18;
            allowPrintQRLabel.Text = "¿Imprimir QR para TA?";
            // 
            // allowLogoLabel
            // 
            allowLogoLabel.AutoSize = true;
            allowLogoLabel.Location = new System.Drawing.Point(171, 277);
            allowLogoLabel.Name = "allowLogoLabel";
            allowLogoLabel.Size = new System.Drawing.Size(81, 13);
            allowLogoLabel.TabIndex = 19;
            allowLogoLabel.Text = "¿Imprimir Logo?";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(12, 440);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(84, 26);
            this.buttonSave.TabIndex = 9;
            this.buttonSave.Text = "Guardar";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonPrinterEvents
            // 
            this.buttonPrinterEvents.Enabled = false;
            this.buttonPrinterEvents.Image = ((System.Drawing.Image)(resources.GetObject("buttonPrinterEvents.Image")));
            this.buttonPrinterEvents.Location = new System.Drawing.Point(330, 74);
            this.buttonPrinterEvents.Name = "buttonPrinterEvents";
            this.buttonPrinterEvents.Size = new System.Drawing.Size(25, 25);
            this.buttonPrinterEvents.TabIndex = 5;
            this.buttonPrinterEvents.UseVisualStyleBackColor = true;
            this.buttonPrinterEvents.Click += new System.EventHandler(this.ButtonPrinterEvents_Click);
            // 
            // buttonReconnectPrinters
            // 
            this.buttonReconnectPrinters.Enabled = false;
            this.buttonReconnectPrinters.Image = ((System.Drawing.Image)(resources.GetObject("buttonReconnectPrinters.Image")));
            this.buttonReconnectPrinters.Location = new System.Drawing.Point(330, 28);
            this.buttonReconnectPrinters.Name = "buttonReconnectPrinters";
            this.buttonReconnectPrinters.Size = new System.Drawing.Size(25, 25);
            this.buttonReconnectPrinters.TabIndex = 3;
            this.buttonReconnectPrinters.UseVisualStyleBackColor = true;
            this.buttonReconnectPrinters.Click += new System.EventHandler(this.ButtonReconnectPrinters_Click);
            // 
            // groupPrinter
            // 
            this.groupPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupPrinter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupPrinter.Controls.Add(this.labelEventsSelected);
            this.groupPrinter.Controls.Add(allowLogoLabel);
            this.groupPrinter.Controls.Add(this.allowLogoCheckBox);
            this.groupPrinter.Controls.Add(allowPrintQRLabel);
            this.groupPrinter.Controls.Add(this.allowPrintQRCheckBox);
            this.groupPrinter.Controls.Add(copiesLabel);
            this.groupPrinter.Controls.Add(this.copiesNumericUpDown);
            this.groupPrinter.Controls.Add(printEventsLabel);
            this.groupPrinter.Controls.Add(this.printEventsListBox);
            this.groupPrinter.Controls.Add(printerLabel);
            this.groupPrinter.Controls.Add(this.printerComboBox);
            this.groupPrinter.Controls.Add(this.buttonReconnectPrinters);
            this.groupPrinter.Controls.Add(this.buttonPrinterEvents);
            this.groupPrinter.Location = new System.Drawing.Point(12, 68);
            this.groupPrinter.Name = "groupPrinter";
            this.groupPrinter.Size = new System.Drawing.Size(361, 366);
            this.groupPrinter.TabIndex = 5;
            this.groupPrinter.TabStop = false;
            // 
            // labelEventsSelected
            // 
            this.labelEventsSelected.AutoSize = true;
            this.labelEventsSelected.Location = new System.Drawing.Point(6, 196);
            this.labelEventsSelected.Name = "labelEventsSelected";
            this.labelEventsSelected.Size = new System.Drawing.Size(116, 13);
            this.labelEventsSelected.TabIndex = 20;
            this.labelEventsSelected.Text = "Tickets seleccionados:";
            // 
            // allowLogoCheckBox
            // 
            this.allowLogoCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.updatePrinterPreferencesRequestBindingSource, "AllowLogo", true));
            this.allowLogoCheckBox.Enabled = false;
            this.allowLogoCheckBox.Location = new System.Drawing.Point(258, 277);
            this.allowLogoCheckBox.Name = "allowLogoCheckBox";
            this.allowLogoCheckBox.Size = new System.Drawing.Size(15, 14);
            this.allowLogoCheckBox.TabIndex = 7;
            this.allowLogoCheckBox.UseVisualStyleBackColor = true;
            // 
            // allowPrintQRCheckBox
            // 
            this.allowPrintQRCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.updatePrinterPreferencesRequestBindingSource, "AllowPrintQR", true));
            this.allowPrintQRCheckBox.Enabled = false;
            this.allowPrintQRCheckBox.Location = new System.Drawing.Point(121, 326);
            this.allowPrintQRCheckBox.Name = "allowPrintQRCheckBox";
            this.allowPrintQRCheckBox.Size = new System.Drawing.Size(13, 13);
            this.allowPrintQRCheckBox.TabIndex = 8;
            this.allowPrintQRCheckBox.UseVisualStyleBackColor = true;
            // 
            // copiesNumericUpDown
            // 
            this.copiesNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.updatePrinterPreferencesRequestBindingSource, "Copies", true));
            this.copiesNumericUpDown.Enabled = false;
            this.copiesNumericUpDown.Location = new System.Drawing.Point(121, 277);
            this.copiesNumericUpDown.Name = "copiesNumericUpDown";
            this.copiesNumericUpDown.Size = new System.Drawing.Size(34, 20);
            this.copiesNumericUpDown.TabIndex = 6;
            // 
            // printEventsListBox
            // 
            this.printEventsListBox.FormattingEnabled = true;
            this.printEventsListBox.Location = new System.Drawing.Point(121, 74);
            this.printEventsListBox.Name = "printEventsListBox";
            this.printEventsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.printEventsListBox.Size = new System.Drawing.Size(184, 95);
            this.printEventsListBox.TabIndex = 4;
            // 
            // printerComboBox
            // 
            this.printerComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.updatePrinterPreferencesRequestBindingSource, "Printer", true));
            this.printerComboBox.Enabled = false;
            this.printerComboBox.FormattingEnabled = true;
            this.printerComboBox.Location = new System.Drawing.Point(121, 28);
            this.printerComboBox.Name = "printerComboBox";
            this.printerComboBox.Size = new System.Drawing.Size(184, 21);
            this.printerComboBox.TabIndex = 2;
            // 
            // nameTextBox
            // 
            this.nameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.updatePrinterPreferencesRequestBindingSource, "Name", true));
            this.nameTextBox.Enabled = false;
            this.nameTextBox.Location = new System.Drawing.Point(133, 24);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(184, 20);
            this.nameTextBox.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // buttonCloseUpdatePreferences
            // 
            this.buttonCloseUpdatePreferences.Location = new System.Drawing.Point(289, 440);
            this.buttonCloseUpdatePreferences.Name = "buttonCloseUpdatePreferences";
            this.buttonCloseUpdatePreferences.Size = new System.Drawing.Size(84, 26);
            this.buttonCloseUpdatePreferences.TabIndex = 10;
            this.buttonCloseUpdatePreferences.Text = "Cerrar";
            this.buttonCloseUpdatePreferences.UseVisualStyleBackColor = true;
            this.buttonCloseUpdatePreferences.Click += new System.EventHandler(this.ButtonCloseUpdatePreferences_Click);
            // 
            // updatePrinterPreferencesRequestBindingSource
            // 
            this.updatePrinterPreferencesRequestBindingSource.DataSource = typeof(Menoo.PrinterService.Client.DTOs.UpdatePrinterPreferencesRequest);
            // 
            // UpdatePreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(385, 476);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCloseUpdatePreferences);
            this.Controls.Add(nameLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupPrinter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePreferences";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Preferences_FormClosed);
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.groupPrinter.ResumeLayout(false);
            this.groupPrinter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.copiesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updatePrinterPreferencesRequestBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonPrinterEvents;
        private System.Windows.Forms.Button buttonReconnectPrinters;
        private System.Windows.Forms.GroupBox groupPrinter;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.NumericUpDown copiesNumericUpDown;
        private System.Windows.Forms.ListBox printEventsListBox;
        private System.Windows.Forms.ComboBox printerComboBox;
        private System.Windows.Forms.CheckBox allowLogoCheckBox;
        private System.Windows.Forms.CheckBox allowPrintQRCheckBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button buttonCloseUpdatePreferences;
        private System.Windows.Forms.BindingSource updatePrinterPreferencesRequestBindingSource;
        private System.Windows.Forms.Label labelEventsSelected;
    }
}