using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business.Settings
{
    public static class PrintEvents
    {
        public static string NEW_BOOKING = "NEW_BOOKING";

        public static string NEW_TABLE_ORDER = "NEW_TABLE_ORDER";

        public static string NEW_TAKE_AWAY = "NEW_TAKE_AWAY";

        public static string ORDER_CANCELLED = "ORDER_CANCELLED";

        public static string TABLE_CLOSED = "TABLE_CLOSED";

        public static string TABLE_OPENED = "TABLE_OPENED";
    }

    [FirestoreData]
    public class PrintSettings
    {
        public List<Settings> Settings { get; set; }
    }

    [FirestoreData]
    public class Settings
    {
        [FirestoreProperty("allowPrinting")]

        public bool AllowPrinting { get; set; }

        [FirestoreProperty("copies")]
        public string Copies { get; set; }

        [FirestoreProperty("id")]
        public string Id { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("printer")]
        public string Printer { get; set; }

        [FirestoreProperty("printEvents")]
        public List<string> PrintEvents { get; set; }

        [FirestoreProperty("printQR")]
        public bool PrintQR { get; set; }

        [FirestoreProperty("updatedAt")]
        public string UpdatedAt { get; set; }
    }
}
