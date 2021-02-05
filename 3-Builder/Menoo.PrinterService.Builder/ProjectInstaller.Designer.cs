
namespace Menoo.PrinterService.Builder
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

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerInstance = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerInstance = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstallerInstance.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerInstance.Password = null;
            this.serviceProcessInstallerInstance.Username = null;

            this.serviceInstallerInstance.DelayedAutoStart = true;
            this.serviceInstallerInstance.Description = @"Menoo.Printer.Builder es un servicio para construir el ticket de impresión.";
            this.serviceInstallerInstance.DisplayName = @"Menoo.Printer.Builder";
            this.serviceInstallerInstance.ServiceName = @"Menoo.Printer.Builder";
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