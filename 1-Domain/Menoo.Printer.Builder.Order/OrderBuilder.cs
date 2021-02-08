using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Constants;
using Menoo.Printer.Builder.Orders.Extensions;
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

        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly string _queueConnectionString;

        private readonly int _queueDelay;

        private readonly string _queueName;

        private readonly BookingRepository _bookingRepository;

        private readonly PaymentRepository _paymentRepository;

        private readonly OrderRepository _orderRepository;

        private readonly ItemRepository _itemRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public OrderBuilder(
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            PaymentRepository paymentRepository,
            BookingRepository bookingRepository,
            OrderRepository orderRepository,
            ItemRepository itemRepository)
        {
            _firestoreDb = firestoreDb;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _adapter = new BuiltinHandlerActivator();
            _queueName = GlobalConfig.ConfigurationManager.GetSetting("queueName");
            _queueConnectionString = GlobalConfig.ConfigurationManager.GetSetting("queueConnectionString");
            _queueDelay = int.Parse(GlobalConfig.ConfigurationManager.GetSetting("queueDelay"));
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public void Build()
        {
            _adapter.Handle<PrintMessage>(async message =>
            {
                await RecieveAsync(message);
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
            string type = !string.IsNullOrEmpty(data.SubTypeDocument) ? $"{data.TypeDocument}-{data.SubTypeDocument}" : $"{data.TypeDocument}";
            _generalWriter.WriteEntry(
                $"OrderBuilder::RecieveAsync(). Nuevo ticket de impresión recibido. {Environment.NewLine}" +
                $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                $"Tipo: {type}{Environment.NewLine}" +
                $"FirebaseId: {data.DocumentId}{Environment.NewLine}", EventLogEntryType.Information);
            if (data.TypeDocument != PrintTypes.ORDER)
            {
                return;
            }
            try
            {
                var orderDTO = await _orderRepository.GetOrderById(data.DocumentId);
                if (data.PrintEvent == PrintEvents.NEW_ORDER || data.PrintEvent == PrintEvents.NEW_TAKE_AWAY)
                {
                    BuildOrderCreated(orderDTO, data.SubTypeDocument);
                }
                else if (data.PrintEvent == PrintEvents.ORDER_CANCELLED)
                {
                    BuildOrderCancelled(orderDTO);
                }
            }
            catch (Exception e) 
            {
                _generalWriter.WriteEntry(
                    $"OrderBuilder::RecieveAsync(). NO se imprimió el ticket de impresión recibido. {Environment.NewLine}" +
                    $"Evento: {data.PrintEvent}{Environment.NewLine}" +
                    $"Tipo: {type}{Environment.NewLine}" +
                    $"FirebaseId: {data.DocumentId}{Environment.NewLine}" +
                    $"Excepción: {e.ToString()}", EventLogEntryType.Error);
            }
            await Task.Delay(_queueDelay);
        }

        public override string ToString()
        {
            return "Order.Builder";
        }

        #region private methods
        private void BuildOrderCreated(OrderV2 order, string orderType)
        {
            List<PrintSettings> sectors = new List<PrintSettings>();
            var sectorsByItems = ItemExtensions.GetPrintSector(order.Items, _itemRepository);
            bool isTakeAway = !string.IsNullOrEmpty(orderType) && orderType.Contains("TakeAway");
            string printEvent = string.Empty;
            if (isTakeAway)
            {
                printEvent = PrintEvents.NEW_TAKE_AWAY;
            }
            else if (order.OrderType.ToUpper().Trim() == OrderTypes.RESERVA)
            {
                printEvent = PrintEvents.NEW_BOOKING;
            }
            else if (order.OrderType.ToUpper().Trim() == OrderTypes.MESA) 
            {
                printEvent = PrintEvents.NEW_TABLE_ORDER;
            }
            if (sectorsByItems.Count > 0)
            {
                if (sectorsByItems.Select(s => s.Sectors).Count() > 0)
                {
                    foreach (var sector in sectorsByItems.Select(s => s.Sectors).FirstOrDefault())
                    {
                        bool isExists = sectors.Any(f => sector.Name == f.Name);
                        if (!isExists && sector.AllowPrinting)
                        {
                            sectors.Add(sector);
                        }
                    }
                }
                if (isTakeAway)
                {
                    var sectorByEvents = GetSectorByEvent(order.Store.Id, printEvent);
                    if (sectorByEvents.Count > 0)
                    {
                        foreach (var sector in sectorByEvents)
                        {
                            bool isExists = sectors.Any(f => sector.Name == f.Name);
                            if (!isExists && sector.AllowPrinting)
                            {
                                sectors.Add(sector);
                            }
                        }
                    }
                }
            }
            else
            {
                sectors.AddRange(GetSectorByEvent(order.Store.Id, printEvent));
            }
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.OrderBy(o => o.Name))
                {
                    SaveOrderAsync(order, sector.Name, sector.Copies, sector.Printer, false, isTakeAway, sector.PrintQR).GetAwaiter().GetResult();
                }
            }
        }

        private void BuildOrderCancelled(OrderV2 order)
        {
            bool isTakeAway = order.OrderType.ToUpper().Trim() == OrderTypes.TAKEAWAY;
            List<PrintSettings> sectors = GetSectorByEvent(order.Store.Id, PrintEvents.ORDER_CANCELLED);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.Where(f => f.AllowPrinting).OrderBy(o => o.Name))
                {
                    SaveOrderAsync(order, sector.Name, sector.Copies, sector.Printer, true, isTakeAway, sector.PrintQR).GetAwaiter().GetResult();
                }
            }
        }

        private string CreateHtmlFromLines(OrderV2 order)
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

        private async Task CreateOrderTicket(OrderV2 order, Ticket ticket, string line, bool isOrderOk, bool isCancelled = false, bool isTakeAway = false, bool printQR = false)
        {
            StringBuilder builder = new StringBuilder();
            string qrCode = "";
            string title;
            if (isTakeAway && printQR && !isCancelled)
            {
                qrCode = GenerateOrderQR(order);
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
                title = isCancelled ? "Orden de reserva cancelada" : "Nueva orden de reserva";
                ticket.SetOrder(title, builder.ToString());
            }
            else if (isOrderOk && isTakeAway)
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

        private List<PrintSettings> GetSectorByEvent(string storeId, string @event)
        {
            var storeData = _storeRepository.GetById<Store>(storeId, "stores").GetAwaiter().GetResult();
            return storeData.GetPrintSettings(@event);
        }

        private async Task SaveOrderAsync(OrderV2 order, string sectorName, int copies, string printerName, bool isCancelled, bool isTakeAway, bool printQR)
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
            ticket.PrintBefore = isTakeAway ? Utils.BeforeAt(order.OrderDate, 5) : Utils.BeforeAt(order.OrderDate, 10);
            var line = CreateHtmlFromLines(order);
            await CreateOrderTicket(order, ticket, line, isOrderOk, isCancelled, isTakeAway, printQR);
            _generalWriter.WriteEntry($"OrderBuilder::SaveOrderAsync(). Enviando a imprimir la orden con la siguiente información.{Environment.NewLine}Detalles:{Environment.NewLine}" +
                $"Nombre de la impresora: {printerName}{Environment.NewLine}" +
                $"Sector de impresión: {sectorName}{Environment.NewLine}" +
                $"Restaurante: {ticket.StoreName}{Environment.NewLine}" + 
                $"Número de orden: {order.OrderNumber}{Environment.NewLine}" +
                $"Tipo de orden: {order.OrderType.ToUpper().Trim()}" +
                $"Estado de la orden: {order.Status.ToUpper()}");
            await _ticketRepository.SaveAsync(ticket);
        }
        #endregion
    }
}
