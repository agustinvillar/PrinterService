using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Constants
{
    public static class PrintTypes
    {
        public static string ORDER = "Orden";

        public static string BOOKING = "Reserva";

        public static string TABLE = "Mesa";

        public static string REQUEST_PAYMENT = "Solicitud de pago";
    }

    public static class SubOrderPrintTypes 
    {
        public static string ORDER_TA = "TakeAway";

        public static string ORDER_TABLE = "Mesa";

        public static string ORDER_BOOKING = "Reserva";

        public static string TABLE_OPENING = "Apertura de mesa";

        public static string TABLE_CLOSE = "Cierre de mesa";

        public static string REQUEST_PAYMENT_POS = "POS";

        public static string REQUEST_PAYMENT_CASH = "Efectivo";
    }
}
