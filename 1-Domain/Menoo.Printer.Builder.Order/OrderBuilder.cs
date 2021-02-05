using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Constants;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Enums;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using QRCoder;
using Rebus.Activation;
using Rebus.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Orders
{
    [Handler]
    public class OrderBuilder : ITicketBuilder, ISubscriptionService, IDisposable
    {
        private readonly BuiltinHandlerActivator _adapter;

        private readonly BookingRepository _bookingRepository;

        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly PaymentRepository _paymentRepository;

        private readonly string _queueConnectionString;

        private readonly int _queueDelay;

        private readonly string _queueName;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public OrderBuilder(
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            PaymentRepository paymentRepository,
            BookingRepository bookingRepository,
            EventLog generalWriter)
        {
            _firestoreDb = firestoreDb;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _adapter = new BuiltinHandlerActivator();
            _generalWriter = generalWriter;
        }

        public void Build(PrintMessage message)
        {
            _adapter.Handle<PrintMessage>(async job =>
            {
                await RecieveAsync(job);
            });

            Configure.With(_adapter)
                .Logging(l => l.Serilog())
                .Transport(t => t.UseRabbitMq(_queueConnectionString, _queueName))
                .Options(o => o.SetMaxParallelism(1))
                .Options(o => o.SetNumberOfWorkers(1))
                .Start();

            _adapter.Bus.Subscribe<PrintMessage>().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
            }
        }

        public async Task RecieveAsync(PrintMessage data, Dictionary<string, string> extras = null)
        {

            await Task.Delay(_queueDelay);
        }

        #region private methods
        private static string CreateHtmlFromLines(OrderV2 order)
        {
            var lines = new List<string>();
            if (order.Items == null)
            {
                return string.Empty;
            }
            foreach (var item in order.Items)
            {
                if (item != null)
                {
                    lines.Add($"<p style='font-size: 65px;'><b>--{item.Name}</b> x {item.Quantity}</p>");
                }

                if (item?.Options != null)
                {
                    lines.AddRange(item.Options.Select(option => option.Name));
                }
                if (!string.IsNullOrEmpty(item?.GuestComment))
                {
                    lines.Add($"Comentario: {item.GuestComment}");
                }
            }
            string items = lines.Aggregate(string.Empty, (current, line) => current + ($"<p>{line}</p>"));
            return items;
        }

        private static bool IsTakeAway(OrderV2 order, bool orderOk)
        {
            return orderOk && order.IsTakeAway;
        }

        private async Task CreateOrderTicket(OrderV2 order, Ticket ticket, string line, bool isOrderOk, bool isCancelled = false, bool printQR = false)
        {
            StringBuilder builder = new StringBuilder();
            string qrCode = "";
            string title;
            if (!isCancelled)
            {
                if (order.IsTakeAway && printQR)
                {
                    qrCode = GenerateOrderQR(order);
                }
            }
            if (isOrderOk && order.OrderType.ToUpper().Trim() == OrderTypes.MESA)
            {
                builder.Append(@"<table class=""top"">");
                builder.Append("<tr>");
                builder.Append("<td>Cliente: </td>");
                builder.Append($"<td>{order.UserName}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Servir en mesa: </td>");
                builder.Append($"<td>{order.Address}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Número de orden: </td>");
                builder.Append($"<td>{order.OrderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                builder.Append(line);
                builder.Append(qrCode);
                title = isCancelled ? "Orden de mesa cancelada" : "Nueva orden de mesa";
                ticket.SetOrder(title, builder.ToString());
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == OrderTypes.RESERVA)
            {
                var bookingData = await _bookingRepository.GetById<Booking>(order.BookingId);
                builder.Append(@"<table class=""top"">");
                builder.Append("<tr>");
                builder.Append("<td>Número de reserva : </td>");
                builder.Append($"<td>{bookingData.BookingNumber}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Fecha: </td>");
                builder.Append($"<td>{order.OrderDate}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Cliente: </td>");
                builder.Append($"<td>{order.UserName}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Número de orden: </td>");
                builder.Append($"<td>{order.OrderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                builder.Append(line);
                builder.Append(qrCode);
                title = isCancelled ? "Orden de reserva" : "Nueva orden de reserva";
                ticket.SetOrder(title, builder.ToString());
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == OrderTypes.TAKEAWAY)
            {
                var payment = await _paymentRepository.GetPayment(order.TableOpeningId, OrderTypes.TAKEAWAY);
                builder.Append(@"<table class=""top"">");
                builder.Append("<tr>");
                builder.Append("<td>Cliente: </td>");
                builder.Append($"<td>{order.UserName}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Hora de retiro: </td>");
                builder.Append($"<td>{order.TakeAwayHour}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Número de orden: </td>");
                builder.Append($"<td>{order.OrderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                if (payment != null)
                {
                    builder.Append(line);
                    builder.Append($"<p>Método de Pago: {payment.PaymentMethod}</p>");
                    builder.Append($"<p>--------------------------------------------------</p>");
                    if (!isCancelled)
                    {
                        builder.Append($"<p>Recuerde <b>ACEPTAR</b> el pedido.</p>");
                        builder.Append($"<p>Pedido <b>YA PAGO</b>.</p>");
                    }
                    else
                    {
                        builder.Append($"<p>Pedido <b>CANCELADO</b>.</p>");
                    }
                    builder.Append($"<p>--------------------------------------------------</p>");
                    builder.Append(@"<div class=""center""><b>TOTAL: $" + payment.TotalToPayTicket + "</b></div>");
                }
                else
                {
                    builder.Append(line);
                }
                builder.Append(qrCode);
                title = isCancelled ? "TakeAway cancelado" : "Nuevo TakeAway";
                ticket.SetOrder(title, builder.ToString());
            }
            builder.Clear();
        }

        private string GenerateOrderQR(OrderV2 order)
        {
            OrderQR orderInfoQR = new OrderQR
            {
                OrderId = order.Id,
                OrderType = order.OrderType.Trim().Capitalize(),
                StoreId = order.Store.Id
            };
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(orderInfoQR, Formatting.Indented), QRCodeGenerator.ECCLevel.Q);
            var imgType = Base64QRCode.ImageType.Jpeg;
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = qrCode.GetGraphic(20, Color.Black, Color.White, true, imgType);
            string htmlPictureTag = $"<img alt=\"Embedded QR Code\" src=\"data:image/{imgType.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" style=\"width: 50%; height: 50%\"/>";
            return htmlPictureTag;
        }

        private List<PrintSettings> GetSectorByEvent(OrderV2 order)
        {
            string printEvent = "";
            string orderType = order.OrderType.ToUpper().Trim();
            if (orderType == OrderTypes.MESA)
            {
                printEvent = PrintEvents.NEW_TABLE_ORDER;
            }
            else if (orderType == OrderTypes.TAKEAWAY)
            {
                printEvent = PrintEvents.NEW_TAKE_AWAY;
            }
            else if (orderType == OrderTypes.RESERVA)
            {
                printEvent = PrintEvents.NEW_BOOKING;
            }
            var storeData = _storeRepository.GetById<Store>("stores", order.Store.Id).GetAwaiter().GetResult();
            return storeData.GetPrintSettings(printEvent);
        }

        private async Task SaveOrderAsync(OrderV2 order, int copies, string printerName, bool isCancelled, bool printQR)
        {
            var ticket = new Ticket
            {
                StoreId = order.Store.Id,
                StoreName = order.Store.Name,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = isCancelled ? TicketTypeEnum.CANCELLED_ORDER.ToString() : TicketTypeEnum.ORDER.ToString(),
                PrinterName = printerName,
                Copies = copies
            };
            bool isOrderOk = order?.OrderType != null;
            if (IsTakeAway(order, isOrderOk))
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, -5);
            }
            else
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, 30);
            }
            var line = CreateHtmlFromLines(order);
            await CreateOrderTicket(order, ticket, line, isOrderOk, isCancelled, printQR);
            await _ticketRepository.SaveAsync<Ticket>(ticket);
        }
        #endregion
    }
}
