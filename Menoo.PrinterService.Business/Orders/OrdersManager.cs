using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Menoo.PrinterService.Business.Entities.Ticket;

namespace Menoo.PrinterService.Business.Orders
{
    /// <summary>
    /// Maneja los eventos de la colección orders
    /// </summary>
    public class OrdersManager
    {
        private const string MESA = "MESAS";

        private const string RESERVA = "RESERVA";

        private const string TAKEAWAY = "TAKEAWAY";

        private readonly FirestoreDb _db;

        public OrdersManager(FirestoreDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Escucha los eventos disparados en la colección orders y orderFamily.
        /// </summary>
        public void Listen()
        {
            _db.Collection("orders")
               .OrderByDescending("orderNumber")
               .Limit(1)
               .Listen(OnCreated);

            _db.Collection("orders")
                .WhereEqualTo("status", "cancelado")
                .Listen(OnCancelled);
        }

        #region events

         /// <summary>
        /// Orden cancelada en la colección 'orders'
        /// </summary>
        /// <param name="snapshot"></param>
        private void OnCancelled(QuerySnapshot snapshot)
        {
            try
            {
                if (snapshot.Documents == null || snapshot.Documents.Count == 0)
                {
                    return;
                }
                var document = snapshot.Documents.Where(f => f.IsCreatedPrinted() && !f.IsCancelledPrinted()).OrderByDescending(o => o.UpdateTime).FirstOrDefault();
                if (document == null) 
                {
                    return;
                }
                var order = document.GetOrderData();
                if (order.OnCreatedPrinted && !order.OnCancelledPrinted)
                {
                    var storeData = Utils.GetStores(_db, order.Store.Id).GetAwaiter().GetResult();
                    var sectors = storeData.GetPrintSettings(PrintEvents.ORDER_CANCELLED);
                    if (sectors.Count > 0)
                    {
                        Utils.SetOrderPrintedAsync(_db, "orders", order.Id, "orderCancelledPrinted").GetAwaiter().GetResult();
                        foreach (var sector in sectors)
                        {
                            if (sector.AllowPrinting)
                            {
                                SaveOrderAsync(order, sector.Copies, sector.Printer, true, sector.PrintQR).GetAwaiter().GetResult();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }

        /// <summary>
        /// Orden creada en la colección 'orders'
        /// </summary>
        private void OnCreated(QuerySnapshot snapshot)
        {
            try
            {
                var document = snapshot.Single();
                if (document == null)
                {
                    return;
                }
                var order = document.GetOrderData();
                if (!order.OnCreatedPrinted && !order.OnCancelledPrinted && order.Status.ToLower() != "cancelado")
                {
                    List<PrintSettings> sectors = new List<PrintSettings>();
                    var sectorsByItems = SectorItemExtensions.GetPrintSector(order.Items, _db);
                    if (sectorsByItems.Count > 0)
                    {
                        if (sectorsByItems.Select(s => s.Sectors).Count() > 0)
                        {
                            foreach (var sector in sectorsByItems.Select(s => s.Sectors).FirstOrDefault())
                            {
                                if (!sectors.Any(f => sector.Name != f.Name) && sector.AllowPrinting)
                                {
                                    sectors.Add(sector);
                                }
                            }
                        }
                        if (order.OrderType.ToUpper().Trim() == TAKEAWAY)
                        {
                            var sectorByEvents = GetSectorByEvent(order);
                            if (sectorByEvents.Count > 0)
                            {
                                sectors.AddRange(sectorByEvents);
                            }
                        }
                    }
                    else
                    {
                        sectors.AddRange(GetSectorByEvent(order));
                    }
                    if (sectors.Count > 0)
                    {
                        Utils.SetOrderPrintedAsync(_db, "orders", order.Id, "orderCreatedPrinted").GetAwaiter().GetResult();
                        foreach (var sector in sectors.OrderBy(o => o.Name))
                        {
                             SaveOrderAsync(order, sector.Copies, sector.Printer, false, sector.PrintQR).GetAwaiter().GetResult();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }
        #endregion

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
            if (isOrderOk && order.OrderType.ToUpper().Trim() == MESA)
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
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == RESERVA)
            {
                var bookingData = Utils.GetDocument(_db, "bookings", order.BookingId).GetAwaiter().GetResult().ConvertTo<Booking>();
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
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == TAKEAWAY)
            {
                var payment = await GetPayment(order.TableOpeningId, TAKEAWAY);
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
                title = isCancelled ? "Takeaway cancelado" : "Nuevo TakeAway";
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

        private async Task<Payment> GetPayment(string id, string type)
        {
            string filter = type == MESA ? "tableOpening.id" : "taOpening.id";
            var documentSnapshots = await _db.Collection("payments").WhereEqualTo(filter, id).GetSnapshotAsync();
            var payments = documentSnapshots.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
            return payments.FirstOrDefault();
        }

        private async Task SaveOrderAsync(OrderV2 order, int copies, string printerName, bool isCancelled, bool printQR)
        {
            var ticket = new Ticket {
                StoreId = order.Store.Id,
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
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private List<PrintSettings> GetSectorByEvent(OrderV2 order) 
        {
            string printEvent = "";
            string orderType = order.OrderType.ToUpper().Trim();
            if (orderType == MESA)
            {
                printEvent = PrintEvents.NEW_TABLE_ORDER;
            }
            else if (orderType == TAKEAWAY)
            {
                printEvent = PrintEvents.NEW_TAKE_AWAY;
            }
            else if (orderType == RESERVA)
            {
                printEvent = PrintEvents.NEW_BOOKING;
            }
            Store storeData = Utils.GetStores(_db, order.Store.Id).GetAwaiter().GetResult();
            return storeData.GetPrintSettings(printEvent);
        }
        #endregion
    }
}
