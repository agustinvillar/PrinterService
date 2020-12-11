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
        public string Data { get; private set}
        
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
    }
}
