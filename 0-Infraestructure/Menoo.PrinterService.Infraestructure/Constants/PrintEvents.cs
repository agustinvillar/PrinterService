using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Constants
{
    public static class PrintEvents
    {
        public static string NEW_BOOKING = "NEW_BOOKING";

        public static string NEW_TABLE_ORDER = "NEW_TABLE_ORDER";

        public static string NEW_TAKE_AWAY = "NEW_TAKE_AWAY";

        public static string NEW_ORDER = "NEW_ORDER";

        public static string ORDER_CANCELLED = "ORDER_CANCELLED";

        public static string TABLE_CLOSED = "TABLE_CLOSED";

        public static string TABLE_OPENED = "TABLE_OPENED";

        public static string CANCELED_BOOKING = "CANCELED_BOOKING";

        public static string REQUEST_PAYMENT = "REQUEST_PAYMENT";

        public static string REPRINT_ORDER = "REPRINT_ORDER";

        public static bool EventExists(string printEvent) 
        {
            var eventList = new List<string>() { NEW_BOOKING, NEW_TABLE_ORDER, NEW_TAKE_AWAY, ORDER_CANCELLED, TABLE_CLOSED, TABLE_OPENED, CANCELED_BOOKING, REQUEST_PAYMENT, REPRINT_ORDER };
            return eventList.Contains(printEvent);
        }
    }
}
