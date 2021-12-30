using System.Collections.Generic;

namespace Menoo.PrinterService.Infraestructure.Constants
{
    public static class PrintEvents
    {
        public const string NEW_BOOKING = "NEW_BOOKING";

        public const string NEW_TABLE_ORDER = "NEW_TABLE_ORDER";

        public const string NEW_TABLE_ORDER_ITEM = "NEW_TABLE_ORDER_ITEM";

        public const string NEW_TAKE_AWAY = "NEW_TAKE_AWAY";

        public const string NEW_ORDER = "NEW_ORDER";

        public const string ORDER_CANCELLED = "ORDER_CANCELLED";

        public const string TABLE_CLOSED = "TABLE_CLOSED";

        public const string TABLE_OPENED = "TABLE_OPENED";

        public const string CANCELED_BOOKING = "CANCELED_BOOKING";

        public const string REQUEST_PAYMENT = "REQUEST_PAYMENT";

        public const string REPRINT_ORDER = "REPRINT_ORDER";

        public static bool EventExists(string printEvent) 
        {
            var eventList = new List<string>() { NEW_BOOKING, NEW_TABLE_ORDER, NEW_TAKE_AWAY, ORDER_CANCELLED, TABLE_CLOSED, TABLE_OPENED, CANCELED_BOOKING, REQUEST_PAYMENT, REPRINT_ORDER };
            return eventList.Contains(printEvent);
        }
    }
}
