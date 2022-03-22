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
            this.labelStore = new System.Windows.Forms.Label();
            this.comboStore = new System.Windows.Forms.ComboBox();
            this.groupPrinter = new System.Windows.Forms.GroupBox();
            this.labelCopies = new System.Windows.Forms.Label();
            this.textCopies = new System.Windows.Forms.TextBox();
            this.checkBoxPrintQR = new System.Windows.Forms.CheckBox();
            this.labelPrintEvents = new System.Windows.Forms.Label();
            this.comboPrintEvents = new System.Windows.Forms.ComboBox();
            this.checkBoxPrintLogo = new System.Windows.Forms.CheckBox();
            this.labelPrintLogo = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupStore.SuspendLayout();
            this.groupPrinter.SuspendLayout();
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
            this.textSectorName.Size = new System.Drawing.Size(204, 20);
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
            this.comboPrinters.Size = new System.Drawing.Size(198, 21);
            this.comboPrinters.TabIndex = 3;
            // 
            // groupStore
            // 
            this.groupStore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupStore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupStore.CausesValidation = false;
            this.groupStore.Controls.Add(this.comboStore);
            this.groupStore.Controls.Add(this.labelStore);
            this.groupStore.Controls.Add(this.labelSectorName);
            this.groupStore.Controls.Add(this.textSectorName);
            this.groupStore.Location = new System.Drawing.Point(12, 12);
            this.groupStore.Name = "groupStore";
            this.groupStore.Size = new System.Drawing.Size(342, 116);
            this.groupStore.TabIndex = 4;
            this.groupStore.TabStop = false;
            this.groupStore.Text = "Datos básicos";
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
            // comboStore
            // 
            this.comboStore.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStore.FormattingEnabled = true;
            this.comboStore.Location = new System.Drawing.Point(121, 30);
            this.comboStore.Name = "comboStore";
            this.comboStore.Size = new System.Drawing.Size(204, 21);
            this.comboStore.TabIndex = 4;
            // 
            // groupPrinter
            // 
            this.groupPrinter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPrinter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupPrinter.Controls.Add(this.labelPrintLogo);
            this.groupPrinter.Controls.Add(this.checkBoxPrintLogo);
            this.groupPrinter.Controls.Add(this.comboPrintEvents);
            this.groupPrinter.Controls.Add(this.labelPrintEvents);
            this.groupPrinter.Controls.Add(this.checkBoxPrintQR);
            this.groupPrinter.Controls.Add(this.textCopies);
            this.groupPrinter.Controls.Add(this.labelCopies);
            this.groupPrinter.Controls.Add(this.labelPrinter);
            this.groupPrinter.Controls.Add(this.comboPrinters);
            this.groupPrinter.Location = new System.Drawing.Point(12, 147);
            this.groupPrinter.Name = "groupPrinter";
            this.groupPrinter.Size = new System.Drawing.Size(342, 212);
            this.groupPrinter.TabIndex = 5;
            this.groupPrinter.TabStop = false;
            this.groupPrinter.Text = "Datos de la impresora";
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
            // textCopies
            // 
            this.textCopies.AcceptsTab = true;
            this.textCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCopies.Location = new System.Drawing.Point(121, 73);
            this.textCopies.MaxLength = 10;
            this.textCopies.Name = "textCopies";
            this.textCopies.Size = new System.Drawing.Size(72, 20);
            this.textCopies.TabIndex = 6;
            // 
            // checkBoxPrintQR
            // 
            this.checkBoxPrintQR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxPrintQR.AutoSize = true;
            this.checkBoxPrintQR.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxPrintQR.Location = new System.Drawing.Point(229, 73);
            this.checkBoxPrintQR.Name = "checkBoxPrintQR";
            this.checkBoxPrintQR.Size = new System.Drawing.Size(96, 20);
            this.checkBoxPrintQR.TabIndex = 8;
            this.checkBoxPrintQR.Text = "Imprimir QR";
            this.checkBoxPrintQR.UseMnemonic = false;
            this.checkBoxPrintQR.UseVisualStyleBackColor = true;
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
            // comboPrintEvents
            // 
            this.comboPrintEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPrintEvents.FormattingEnabled = true;
            this.comboPrintEvents.Location = new System.Drawing.Point(121, 124);
            this.comboPrintEvents.Name = "comboPrintEvents";
            this.comboPrintEvents.Size = new System.Drawing.Size(198, 21);
            this.comboPrintEvents.TabIndex = 10;
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
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(279, 379);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 29);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Guardar";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(372, 442);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupPrinter);
            this.Controls.Add(this.groupStore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración sector";
            this.groupStore.ResumeLayout(false);
            this.groupStore.PerformLayout();
            this.groupPrinter.ResumeLayout(false);
            this.groupPrinter.PerformLayout();
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
        private System.Windows.Forms.TextBox textCopies;
        private System.Windows.Forms.CheckBox checkBoxPrintQR;
        private System.Windows.Forms.ComboBox comboPrintEvents;
        private System.Windows.Forms.Label labelPrintEvents;
        private System.Windows.Forms.Label labelPrintLogo;
        private System.Windows.Forms.CheckBox checkBoxPrintLogo;
        private System.Windows.Forms.Button buttonSave;
    }
}