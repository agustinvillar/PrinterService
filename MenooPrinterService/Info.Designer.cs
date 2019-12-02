namespace MenooPrinterService
{
    partial class Info
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Info));
            this.TxtMenoo = new System.Windows.Forms.Label();
            this.TxtVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TxtMenoo
            // 
            this.TxtMenoo.AutoSize = true;
            this.TxtMenoo.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtMenoo.Location = new System.Drawing.Point(11, 21);
            this.TxtMenoo.Name = "TxtMenoo";
            this.TxtMenoo.Size = new System.Drawing.Size(116, 31);
            this.TxtMenoo.TabIndex = 0;
            this.TxtMenoo.Text = "MENOO";
            // 
            // TxtVersion
            // 
            this.TxtVersion.AutoSize = true;
            this.TxtVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtVersion.Location = new System.Drawing.Point(12, 82);
            this.TxtVersion.Name = "TxtVersion";
            this.TxtVersion.Size = new System.Drawing.Size(91, 25);
            this.TxtVersion.TabIndex = 1;
            this.TxtVersion.Text = "MENOO";
            // 
            // Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 128);
            this.Controls.Add(this.TxtVersion);
            this.Controls.Add(this.TxtMenoo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Info";
            this.Text = "lnfo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TxtMenoo;
        private System.Windows.Forms.Label TxtVersion;
    }
}