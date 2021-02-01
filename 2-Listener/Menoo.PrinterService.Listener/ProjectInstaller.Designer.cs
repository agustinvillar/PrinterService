﻿
namespace Menoo.PrinterService.Listener
{
    partial class ProjectInstaller
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        private void InitializeComponent()
        {
            this.serviceProcessInstallerInstance = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerInstance = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstallerInstance.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerInstance.Password = null;
            this.serviceProcessInstallerInstance.Username = null;

            this.serviceInstallerInstance.DelayedAutoStart = true;
            this.serviceInstallerInstance.Description = @"Menoo.Printer.Listener es un servicio para capturar y encolar enventos de impresión procedentes de Firebase.";
            this.serviceInstallerInstance.DisplayName = @"Menoo.Printer.Listener";
            this.serviceInstallerInstance.ServiceName = @"Menoo.Printer.Listener";
            this.serviceInstallerInstance.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[]
            {
                this.serviceProcessInstallerInstance,
                this.serviceInstallerInstance
            });
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerInstance;

        private System.ServiceProcess.ServiceInstaller serviceInstallerInstance;
    }
}