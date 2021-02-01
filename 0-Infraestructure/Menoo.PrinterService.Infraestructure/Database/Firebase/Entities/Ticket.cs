using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Text;

namespace Menoo.PrinterService.Infraestructure.Database.Firebase.Entities
{
    [FirestoreData]
    public class Ticket
    {
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

        [FirestoreProperty("copies")]
        [JsonProperty("copies")]
        public int Copies { get; set; }

        [FirestoreProperty("printData")]
        [JsonProperty("printData")]
        public string Data { get; private set; }

        [FirestoreProperty("date")]
        [JsonProperty("date")]
        public string Date { get; set; }

        [FirestoreProperty("expired")]
        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [FirestoreProperty("itemId")]
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        [FirestoreProperty("printBefore")]
        [JsonProperty("printBefore")]
        public string PrintBefore { get; set; }

        [FirestoreProperty("printed")]
        [JsonProperty("printed")]
        public bool Printed { get; set; }

        [FirestoreProperty("printedAt")]
        [JsonProperty("printedAt")]
        public string PrintedAt { get; set; }

        [FirestoreProperty("printer")]
        [JsonProperty("printer")]
        public string PrinterName { get; set; }

        [FirestoreProperty("storeId")]
        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [FirestoreProperty("ticketType")]
        [JsonProperty("ticketType")]
        public string TicketType { get; set; }

        [FirestoreProperty("tableOpeningFamilyId")]
        [JsonProperty("tableOpeningFamilyId")]
        public string TableOpeningFamilyId { get; set; }

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

        /// <summary>
        /// Inyecta los datos del ticket de solicitud de pago.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="tableNumber">Número de mesa</param>
        /// <param name="date">Fecha de la solicitud.</param>
        /// <param name="data">Información del ticket.</param>
        public void SetRequestPayment(string title, string tableNumber, string date, string total, string data = "")
        {
            string template = Utils.GetTicketTemplate("Ticket_Table_Closing");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@tableNumber", tableNumber);
            builder.Replace("@date", date);
            builder.Replace("@data", data);
            builder.Replace("@total", total);
            Data = builder.ToString();
        }

        /// <summary>
        /// Inyecta los datos del ticket de cierre/abandono de mesa.
        /// </summary>
        /// <param name="title">Titulo del ticket.</param>
        /// <param name="tableNumber">Número de mesa</param>
        /// <param name="date">Fecha del cierre/abandono de mesa.</param>
        /// <param name="data">Información del ticket.</param>
        public void SetTableClosing(string title, string tableNumber, string date, string total, string data = "")
        {
            string template = Utils.GetTicketTemplate("Ticket_Table_Closing");
            var builder = new StringBuilder(template);
            builder.Replace("@title", title);
            builder.Replace("@tableNumber", tableNumber);
            builder.Replace("@date", date);
            builder.Replace("@data", data);
            builder.Replace("@total", total);
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
    }
}
