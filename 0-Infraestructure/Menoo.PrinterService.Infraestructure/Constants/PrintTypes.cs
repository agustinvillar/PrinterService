namespace Menoo.PrinterService.Infraestructure.Constants
{
    public static class PrintTypes
    {
        public const string ORDER = "Orden";

        public const string BOOKING = "Reserva";

        public const string TABLE = "Mesa";

        public const string REQUEST_PAYMENT = "Solicitud de pago";
    }

    public static class SubOrderPrintTypes 
    {
        public const string ORDER_TA = "TakeAway";

        public const string ORDER_TABLE = "Mesa";

        public const string ORDER_BOOKING = "Reserva";

        public const string TABLE_OPENING = "Apertura de mesa";

        public const string TABLE_CLOSE = "Cierre de mesa";

        public const string REQUEST_PAYMENT_POS = "POS";

        public const string REQUEST_PAYMENT_CASH = "Efectivo";
    }
}
