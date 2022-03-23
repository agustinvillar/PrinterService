using System.Windows.Forms;

namespace Menoo.PrinterService.Client.Extensions
{
    public static class ControlsExtensions
    {
        public static void SetToolTip(this Button control, string txt)
        {
            new ToolTip().SetToolTip(control, txt);
        }
    }
}
