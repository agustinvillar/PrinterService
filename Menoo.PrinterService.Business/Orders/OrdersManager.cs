using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
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
            _db.Collection("orderFamily")
               .OrderByDescending("incremental")
               .Limit(1)
               .Listen(OnOrderFamily);

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
                var document = snapshot.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault();
                if (document.IsPrinted())
                {
                    return;
                }
                var order = document.GetOrderData();
                var storeData = Utils.GetStores(_db, order.Store.Id).GetAwaiter().GetResult();
                var sectors = storeData.GetPrintSettings(PrintEvents.ORDER_CANCELLED);
                if (sectors.Count > 0)
                {
                    Utils.SetOrderPrintedAsync(_db, "orders", document.Id).GetAwaiter().GetResult();
                    foreach (var sector in sectors)
                    {
                        if (sector.AllowPrinting)
                        {
                            SaveOrderAsync(order, sector.Copies, sector.Printer).GetAwaiter().GetResult();
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
        /// Orden creada en la colección 'orderFamily'
        /// </summary>
        private void OnOrderFamily(QuerySnapshot snapshot)
        {
            try
            {
                var document = snapshot.Documents.Single();
                var order = document.ToDictionary().GetObject<Order>();
                Store storeData;
                if (order.Store != null)
                {
                    storeData = Utils.GetStores(_db, order.Store.StoreId).GetAwaiter().GetResult();
                }
                else 
                {
                    storeData = Utils.GetStores(_db, order.StoreId).GetAwaiter().GetResult();
                }
                if (order.Printed)
                {
                    return;
                }
                order.Store = storeData;
                order.Id = document.Id;
                var sectors = storeData.GetPrintSettings(PrintEvents.NEW_TABLE_ORDER);
                if (sectors.Count > 0)
                {
                    Utils.SetOrderPrintedAsync(_db, "orderFamily", document.Id).GetAwaiter().GetResult();
                    foreach (var sector in sectors)
                    {
                        if (sector.AllowPrinting)
                        {
                            //Nueva orden 
                            SaveOrderAsync(order, sector.Copies, sector.Printer).GetAwaiter().GetResult();
                            //Código QR
                            if (sector.PrintQR)
                            {
                                GenerateOrderQR(order, sector.Copies, sector.Printer).GetAwaiter().GetResult();
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
        #endregion

        #region private methods
        private static string CreateComments(Order order)
        {
            if (order.Items == null) 
            {
                return string.Empty;
            }
            List<string> lines = new List<string>();
            foreach (var item in order.Items)
            {
                if (item?.CategoryStore != null)
                {
                    lines.Add($"<b>--[{item.CategoryStore?.Name}] {item.Name}</b> x {item.Quantity}");
                }
                else
                {
                    if (item != null)
                    {
                        lines.Add($"<b>--{item.Name}</b> x {item.Quantity}");
                    }
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

        private static string CreateHtmlFromLines(OrderCancelled order)
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

        private static bool IsTakeAway(Order order, bool orderOk)
        {
            return orderOk && order.IsTakeAway;
        }

        private void CreateOrderTicket(OrderCancelled order, Ticket ticket, string line, string orderType)
        {
            StringBuilder builder = new StringBuilder();
            switch (orderType.ToUpper())
            {
                case nameof(RESERVA):
                    var bookingData = Utils.GetDocument(_db, "bookings", order.BookingId).GetAwaiter().GetResult().ConvertTo<Booking>();
                    builder.Append(@"<table class=""top"">");
                    builder.Append("<tr>");
                    builder.Append("<td>Cliente: </td>");
                    builder.Append($"<td>{order.UserName}</td>");
                    builder.Append("</tr>");
                    builder.Append("<tr>");
                    builder.Append("<td>Número de reserva : </td>");
                    builder.Append($"<td>{bookingData.BookingNumber}</td>");
                    builder.Append("</tr>");
                    builder.Append("</table>");
                    if (!string.IsNullOrEmpty(order.OrderNumber)) 
                    {
                        builder.Append("</tr>");
                        builder.Append("<td>Número de orden: </td>");
                        builder.Append($"<td>{order.OrderNumber}</td>");
                        builder.Append("</tr>");
                    }
                    builder.Append(line);
                    ticket.SetOrder("Orden reserva cancelada", builder.ToString());
                    break;
                case nameof(TAKEAWAY):
                    builder.Append(@"<table class=""top"">");
                    builder.Append("<tr>");
                    builder.Append("<td>Cliente: </td>");
                    builder.Append($"<td>{order.UserName}</td>");
                    builder.Append("</tr>");
                    builder.Append("<tr>");
                    builder.Append("<td>Hora de retiro: </td>");
                    builder.Append($"<td>{order.TakeAwayHour}</td>");
                    builder.Append("</tr>");
                    builder.Append("</table>");
                    if (!string.IsNullOrEmpty(order.OrderNumber))
                    {
                        builder.Append("<tr>");
                        builder.Append("<td>Número de orden: </td>");
                        builder.Append($"<td>{order.OrderNumber}</td>");
                        builder.Append("</tr>");
                    }
                    builder.Append(line);
                    ticket.SetOrder("Orden TakeAway cancelada", builder.ToString());
                    break;
                default:
                    string orderNumber = GetOrderNumber(order.TableOpeningFamilyId).GetAwaiter().GetResult();
                    builder.Append(@"<table class=""top"">");
                    builder.Append("<tr>");
                    builder.Append("<td>Cliente: </td>");
                    builder.Append($"<td>{order.UserName}</td>");
                    builder.Append("</tr>");
                    builder.Append("<tr>");
                    builder.Append("<td>Servir en mesa: </td>");
                    builder.Append($"<td>{order.Address}</td>");
                    builder.Append("</tr>");
                    builder.Append("</table>");

                        builder.Append("</tr>");
                        builder.Append("<td>Número de orden: </td>");
                        builder.Append($"<td>{orderNumber}</td>");
                        builder.Append("</tr>");
                    
                    builder.Append(line);
                    ticket.SetOrder("Orden cancelada", builder.ToString());
                    break;
            }
        }

        private async Task CreateOrderTicket(Order order, bool isOrderOk, Ticket ticket, string line)
        {
            StringBuilder builder = new StringBuilder();
            string orderNumber = GetOrderNumber(order.TableOpeningFamilyId).GetAwaiter().GetResult();
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
                builder.Append($"<td>{orderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                builder.Append(line);
                ticket.SetOrder("Nueva orden de mesa", builder.ToString());
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
                builder.Append($"<td>{orderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                builder.Append(line);
                ticket.SetOrder("Nueva orden de reserva", builder.ToString());
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == TAKEAWAY)
            {
                var payment = await GetPayment(order.TableOpeningFamilyId, TAKEAWAY);
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
                builder.Append($"<td>{orderNumber}</td>");
                builder.Append("</tr>");
                builder.Append("</table>");
                if (payment != null)
                {
                    builder.Append(line);
                    builder.Append($"<p>Método de Pago: {payment.PaymentMethod}</p>");
                    builder.Append($"<p>--------------------------------------------------</p>");
                    builder.Append($"<p>Recuerde <b>ACEPTAR</b> el pedido.</p>");
                    builder.Append($"<p>Pedido <b>YA PAGO</b>.</p>");
                    builder.Append($"<p>--------------------------------------------------</p>");
                    builder.Append($"<p>Total: ${payment.TotalToPayTicket}</p>");
                }
                else 
                {
                    builder.Append(line);
                }
                ticket.SetOrder("Nuevo TakeAway", builder.ToString());
            }
            builder.Clear();
        }

        private async Task GenerateOrderQR(Order order, int copies, string printerName)
        {
            string orderType = order.OrderType.ToUpper();
            string imageTag = "<img src='data:image/png;base64, @base64' alt='Order QR'/>";
            var ticket = new Ticket
            {
                StoreId = order.Id,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.ORDER_QR.ToString(),
                PrintBefore = orderType == TAKEAWAY ? Utils.BeforeAt(order.OrderDate, -5) : Utils.BeforeAt(order.OrderDate, 30),
                PrinterName = printerName,
                Copies = copies
            };
            OrderQR orderInfoQR = new OrderQR
            {
                OrderId = order.Id,
                OrderType = order.OrderType.ToUpper().Trim(),
                StoreId = order.StoreId
            };
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(JsonConvert.SerializeObject(orderInfoQR, Formatting.Indented), QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            string qrCodeImageAsBase64 = imageTag.Replace("@base64", qrCode.GetGraphic(20));
            ticket.SetOrder("Código QR Orden", qrCodeImageAsBase64);
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task<string> GetOrderNumber(string tableOpeningFamilyId)
        {
            QuerySnapshot documentSnapshots = await _db.Collection("orders").WhereEqualTo("tableOpeningFamilyId", tableOpeningFamilyId).GetSnapshotAsync();
            if (documentSnapshots.Documents.Count() == 0) 
            {
                documentSnapshots = await _db.Collection("orders").WhereEqualTo("tableOpeningId", tableOpeningFamilyId).GetSnapshotAsync();
            }
            var order = documentSnapshots.Documents.Select(d => d.ToDictionary().GetObject<Order>()).ToList();
            var itemsWithOrderNumber = order.OrderByDescending(o => o.OrderNumber).GroupBy(g => g.OrderNumber);
            if (itemsWithOrderNumber != null) 
            {
                return itemsWithOrderNumber.FirstOrDefault().Key;
            }
            return string.Empty;
        }

        private async Task<Payment> GetPayment(string id, string type)
        {
            string filter = type == MESA ? "tableOpening.id" : "taOpening.id";
            var documentSnapshots = await _db.Collection("payments").WhereEqualTo(filter, id).GetSnapshotAsync();
            var payments = documentSnapshots.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
            return payments.FirstOrDefault();
        }

        private async Task SaveOrderAsync(Order order, int copies, string printerName)
        {
            var ticket = new Ticket 
            {
                StoreId = order.StoreId,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.ORDER.ToString(),
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
            string lines = CreateComments(order);
            await CreateOrderTicket(order, isOrderOk, ticket, lines);
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task SaveOrderAsync(OrderCancelled order, int copies, string printerName)
        {
            var ticket = new Ticket {
                StoreId = order.Store.Id,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.CANCELLED_ORDER.ToString(),
                PrinterName = printerName,
                Copies = copies
            };
            if (order.OrderType.ToUpper() == TAKEAWAY)
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, -5);
            }
            else
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, 30);
            }
            var line = CreateHtmlFromLines(order);
            CreateOrderTicket(order, ticket, line, order.OrderType);
            await Utils.SaveTicketAsync(_db, ticket);
        }
        #endregion
    }
}
