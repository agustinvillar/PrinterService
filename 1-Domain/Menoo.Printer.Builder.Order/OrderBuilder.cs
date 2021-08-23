using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Constants;
using Menoo.Printer.Builder.Orders.Extensions;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.MainSchema;
using Menoo.PrinterService.Infraestructure.Enums;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Newtonsoft.Json;
using QRCoder;
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
    public class OrderBuilder : ITicketBuilder
    {
        private readonly BookingRepository _bookingRepository;

        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly ItemRepository _itemRepository;

        private readonly MenooContext _menooContext;

        private readonly OrderRepository _orderRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        public OrderBuilder(
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            BookingRepository bookingRepository,
            OrderRepository orderRepository,
            ItemRepository itemRepository)
        {
            _firestoreDb = firestoreDb;
            _menooContext = new MenooContext();
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _bookingRepository = bookingRepository;
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task BuildAsync(PrintMessage data)
        {
            if (data.Builder != PrintBuilder.ORDER_BUILDER)
            {
                return;
            }

            if (data.DocumentsId?.Count > 0 && string.IsNullOrEmpty(data.DocumentId))
            {
                var orders = new List<OrderV2>();
                foreach (var documentId in data.DocumentsId)
                {
                    var order = await _orderRepository.GetOrderById(documentId);
                    orders.Add(order);
                }
                BuildMultipleOrderCreated(orders);
            }
            else
            {
                var order = await _orderRepository.GetOrderById(data.DocumentId);
                BuildSingleOrderCreated(order, data);
            }
        }

        public override string ToString()
        {
            return PrintBuilder.ORDER_BUILDER;
        }

        #region private methods
        private void BuildMultipleOrderCreated(List<OrderV2> orders)
        {
            var ordersByStore = orders.GroupBy(g => g.Store.Id).FirstOrDefault();
            var store = _storeRepository.GetById<Store>(ordersByStore.Key, "stores").GetAwaiter().GetResult();
            var unifiedSector = store.SectorUnifiedTicket();
            if (unifiedSector == null)
            {
                return;
            }
            var ordersByOrderNumber = orders.GroupBy(g => g.OrderNumber).FirstOrDefault();
            var ordersByAddress = orders.GroupBy(g => g.Address).FirstOrDefault();
            var ordersByUsername = orders.GroupBy(g => g.UserName).FirstOrDefault();
            var ticket = new Ticket
            {
                StoreId = store.Id,
                StoreName = store.Name,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.ORDER.ToString(),
                PrinterName = unifiedSector.Printer,
                Copies = unifiedSector.Copies,
                PrintBefore = Utils.BeforeAt(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 60)
            };
            string html = CreateUnifiedHtml(ordersByOrderNumber.ToList(), ordersByUsername.Key, ordersByAddress.Key, ordersByOrderNumber.Key);
            ticket.SetOrder("Nueva orden de mesa", html);
            _generalWriter.WriteEntry($"OrderBuilder::SaveOrderAsync(). Enviando a imprimir el ticket unificado con la siguiente información.{Environment.NewLine}Detalles:{Environment.NewLine}" +
                $"Nombre de la impresora: {ticket.PrinterName}{Environment.NewLine}" +
                $"Sector de impresión: {unifiedSector.Name}{Environment.NewLine}" +
                $"Hora de impresión: {ticket.PrintBefore}{Environment.NewLine}" +
                $"Restaurante: {ticket.StoreName}{Environment.NewLine}" +
                $"Número de orden: {ordersByOrderNumber.Key}{Environment.NewLine}");
            _ticketRepository.SaveAsync(ticket).GetAwaiter().GetResult();
            // Imprimir los tickets de forma individual
            if (store.Sectors.Count > 0)
            {
                foreach (var order in orders)
                {
                    BuildOrderCreated(order, OrderTypes.MESA, store.Sectors.Count > 1);
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

        private void BuildOrderCreated(OrderV2 order, string orderType, bool isSingleTicket = true)
        {
            if (!isSingleTicket) 
            {
                return;
            }
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
                var sectorByEvent = GetSectorByEvent(order.Store.Id, printEvent);
                sectors.AddRange(sectorByEvent);
            }
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.OrderBy(o => o.Name))
                {
                    SaveOrderAsync(order, sector.Name, sector.Copies, sector.Printer, false, isTakeAway, sector.PrintQR).GetAwaiter().GetResult();
                }
            }
        }

        private void BuildOrderToReprint(OrderV2 orderDTO)
        {
            BuildOrderCreated(orderDTO, orderDTO.OrderType);
        }

        private void BuildSingleOrderCreated(OrderV2 order, PrintMessage message)
        {
            if (message.PrintEvent == PrintEvents.NEW_ORDER || message.PrintEvent == PrintEvents.NEW_TAKE_AWAY)
            {
                BuildOrderCreated(order, message.SubTypeDocument);
            }
            else if (message.PrintEvent == PrintEvents.ORDER_CANCELLED)
            {
                BuildOrderCancelled(order);
            }
            else if (message.PrintEvent == PrintEvents.REPRINT_ORDER)
            {
                BuildOrderToReprint(order);
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
            string qrCode = string.Empty;
            bool isGuestComments = !string.IsNullOrEmpty(order.GuestComment) && order.IsMarket;
            if (isTakeAway && printQR && !isCancelled)
            {
                qrCode = GenerateOrderQR(order);
            }
            string title;
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
                var payment = _menooContext.Payments.FirstOrDefault(f => f.EntityId == order.Id);
                builder.Append(@"<table class=""top"">");
                builder.Append("<tr>");
                builder.Append("<td>Cliente: </td>");
                builder.Append($"<td>{order.UserName}</td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Hora de retiro: </td>");
                builder.Append($"<td><b>{order.TakeAwayHour}</b></td>");
                builder.Append("</tr>");
                builder.Append("<tr>");
                builder.Append("<td>Número de orden: </td>");
                builder.Append($"<td>{order.OrderNumber}</td>");
                builder.Append("</tr>");
                if (isGuestComments)
                {
                    builder.Append("<tr>");
                    builder.Append("<td>NOTA: </td>");
                    builder.Append($"<td>{order.GuestComment}</td>");
                    builder.Append("</tr>");
                }
                builder.Append("</table>");
                builder.Append(line);
                if (payment != null)
                {
                    var paymentRenglon = payment.Renglones.FirstOrDefault(f => f.Type == PrinterService.Infraestructure.Database.SqlServer.MainSchema.Entities.PaymentRenglon.PaymentRenglonType.DISCOUNT);

                    builder.Append($"<p>Método de Pago: {payment.CardBrand}</p>");
                    builder.Append($"<p>--------------------------------------------------</p>");
                    if (paymentRenglon != null)
                    {
                        builder.Append($"<p>{paymentRenglon.Description}</p>");
                        builder.Append($"<p>--------------------------------------------------</p>");
                    }
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
                    builder.Append(@"<div class=""center""><b>TOTAL: $" + payment.TransactionAmount + "</b><br/><br/><br/><br/></div>");
                }
                builder.Append(qrCode);
                title = isCancelled ? "TakeAway cancelado" : "Nuevo TakeAway";
                ticket.SetOrder(title, builder.ToString());
            }
            builder.Clear();
        }

        private string CreateUnifiedHtml(List<OrderV2> orders, string userName, string address, string orderNumber)
        {
            var builder = new StringBuilder();
            builder.Append(@"<table class=""top"">");
            builder.Append("<tr>");
            builder.Append("<td>Cliente: </td>");
            builder.Append($"<td>{userName}</td>");
            builder.Append("</tr>");
            builder.Append("<tr>");
            builder.Append("<td>Servir en mesa: </td>");
            builder.Append($"<td>{address}</td>");
            builder.Append("</tr>");
            builder.Append("<tr>");
            builder.Append("<td>Número de orden: </td>");
            builder.Append($"<td>{orderNumber}</td>");
            builder.Append("</tr>");
            builder.Append("</table>");
            foreach (var order in orders)
            {
                var line = new StringBuilder();
                if (order.Items == null)
                {
                    line.Append("");
                }
                foreach (var item in order.Items)
                {
                    if (item != null)
                    {
                        line.Append($"<p style='font-size: 65px;'><b>--{item.Name}</b> x {item.Quantity}</p>");
                    }

                    if (item?.Options != null)
                    {
                        for (int i = 0; i < item?.Options.Length; i++)
                        {
                            line.Append(item?.Options[i].Name + "<br>");
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.GuestComment))
                    {
                        line.AppendLine($"Comentario: {item.GuestComment} <br>");
                    }
                }
                string html = $"<p>{line.ToString()}</p><br>";
                builder.Append(html);
            }
            return builder.ToString();
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
            var sectors = storeData.GetPrintSettings(@event);
            return sectors;
        }

        private async Task SaveOrderAsync(OrderV2 order, string sectorName, int copies, string printerName, bool isCancelled, bool isTakeAway, bool printQR)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
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
            ticket.PrintBefore = isTakeAway ? Utils.BeforeAt(now, 30) : Utils.BeforeAt(now, 60);
            var line = CreateHtmlFromLines(order);
            await CreateOrderTicket(order, ticket, line, isOrderOk, isCancelled, isTakeAway, printQR);
            _generalWriter.WriteEntry($"OrderBuilder::SaveOrderAsync(). Enviando a imprimir la orden con la siguiente información.{Environment.NewLine}Detalles:{Environment.NewLine}" +
                $"Nombre de la impresora: {printerName}{Environment.NewLine}" +
                $"Sector de impresión: {sectorName}{Environment.NewLine}" +
                $"Hora de impresión: {ticket.PrintBefore}{Environment.NewLine}" +
                $"Restaurante: {ticket.StoreName}{Environment.NewLine}" +
                $"Número de orden: {order.OrderNumber}{Environment.NewLine}" +
                $"Tipo de orden: {order.OrderType.ToUpper().Trim()}{Environment.NewLine}" +
                $"Estado de la orden: {order.Status.ToUpper()}");
            await _ticketRepository.SaveAsync(ticket);
        }
        #endregion
    }
}
