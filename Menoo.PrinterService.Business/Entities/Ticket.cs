using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using System.Text;

namespace Menoo.PrinterService.Business.Entities
{
    [FirestoreData]
    public class Ticket
    {
        [FirestoreProperty("ticketType")]
        public string TicketType { get; set; }
        
        [FirestoreProperty("printData")]
        public string Data { get; set; }
        
        [FirestoreProperty("printed")]
        public bool Printed { get; set; }
        
        [FirestoreProperty("printedAt")]
        public string PrintedAt { get; set; }
        
        [FirestoreProperty("storeId")]
        public string StoreId { get; set; }
        
        [FirestoreProperty("printBefore")]
        public string PrintBefore { get; set; }
        
        [FirestoreProperty("expired")]
        public bool Expired { get; set; }
        
        [FirestoreProperty("date")]
        public string Date { get; set; }
        
        [FirestoreProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }

        public enum TicketTypeEnum
        {
            OPEN_TABLE,
            CLOSE_TABLE,
            ORDER,
            NEW_BOOKING,
            CANCELLED_BOOKING,
            CANCELLED_ORDER,
            PAYMENT_REQUEST
        }

        /// <summary>
        /// Inyecta los datos del ticket de reserva en la plantilla.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="bookingNumber">Número de reserva.</param>
        /// <param name="date">Fecha de la reserva.</param>
        /// <param name="guestQuantity">Cantidad de personas.</param>
        /// <param name="clientName">Nombre de quien realizó la reserva.</param>
        public void SetBookingData(string title, long bookingNumber, string date, int guestQuantity, string clientName) 
        {
            string template = Utils.GetTicketTemplate("Ticket_Booking");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@bookingNumber", bookingNumber.ToString());
            builder.Replace("@bookingDate", date);
            builder.Replace("@bookingGuestQuantity", guestQuantity.ToString());
            builder.Replace("@clienteName", clientName);
            Data = builder.ToString();
        }

        /// <summary>
        /// Inyecta los datos del ticket de apertura de mesa.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="tableNumber">Número de mesa.</param>
        /// <param name="date">Fecha de la apertura de la mesa.</param>
        public void SetTableOpening(string title, string tableNumber, string date) 
        {
            string template = Utils.GetTicketTemplate("Ticket_Table_Opening");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@tableNumber", tableNumber);
            builder.Replace("@date", date);
            Data = builder.ToString();
        }

        /// <summary>
        /// Inyecta los datos del ticket de cierre/abandono de mesa.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="date">Fecha del cierre/abandono de mesa.</param>
        public void SetTableClosing(string title, string tableNumber, string date, string data = "")
        {
            string template = Utils.GetTicketTemplate("Ticket_Table_Closing");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@tableNumber", tableNumber);
            builder.Replace("@date", date);
            builder.Replace("@data", data);
            Data = builder.ToString();
        }

        /// <summary>
        /// Inyecta los datos del ticket de nueva orden en mesa.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="clientName">Nombre del cliente.</param>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="items">Comida o items que componen la orden</param>
        public void SetOrder(string title, string data) 
        {
            string template = Utils.GetTicketTemplate("Ticket_Order");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@data", data);
            Data = builder.ToString();
        }
    }
}
