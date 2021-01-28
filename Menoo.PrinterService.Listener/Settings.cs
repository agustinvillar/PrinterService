using System.Configuration;

namespace Menoo.PrinterService.Listener
{
    public static class Settings
    {
		public static string DefaultLog
		{
			get
			{
				return string.Empty + ConfigurationManager.AppSettings["DefaultLog"];
			}
		}

		public static string ServiceLog
		{
			get
			{
				return string.Empty + ConfigurationManager.AppSettings["Log"];
			}
		}

		public static string ServiceName
		{
			get
			{
				return string.Empty + ConfigurationManager.AppSettings["ServiceName"];
			}
		}

		public static string ServiceSourceName
		{
			get
			{
				return string.Empty + ConfigurationManager.AppSettings["ServiceSourceName"];
			}
		}
	}
}
