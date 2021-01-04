﻿using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
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
        private const string MESA = "MESA";

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
                if (!storeData.AllowPrint(PrintEvents.ORDER_CANCELLED)) 
                {
                    return;
                }
                Utils.SetOrderPrintedAsync(_db, "orders", document.Id).GetAwaiter().GetResult();
                SaveOrderAsync(order).GetAwaiter().GetResult();
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
                var order = document.ToDictionary().GetObject<Entities.Orders>();
                Store storeData;
                if (order.Store != null)
                {
                    storeData = Utils.GetStores(_db, order.Store.StoreId).GetAwaiter().GetResult();
                }
                else 
                {
                    storeData = Utils.GetStores(_db, order.StoreId).GetAwaiter().GetResult();
                }
                order.Store = storeData;
                order.Id = document.Id;
                if (order.Printed)
                {
                    return;
                }
                if (!storeData.AllowPrint(PrintEvents.NEW_TABLE_ORDER)) 
                {
                    return;
                }
                Utils.SetOrderPrintedAsync(_db, "orderFamily", document.Id).GetAwaiter().GetResult();
                //Nueva orden 
                SaveOrderAsync(order).GetAwaiter().GetResult();
                //Código QR

            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }
        #endregion

        #region private methods
        private static string CreateComments(Entities.Orders order)
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

        private static string CreateHtmlFromLines(OrdersCancelled order)
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

        private static bool IsTakeAway(Entities.Orders order, bool orderOk)
        {
            return orderOk && order.IsTakeAway;
        }

        private void CreateOrderTicket(OrdersCancelled order, Ticket ticket, string line, string orderType)
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

        private async Task CreateOrderTicket(Entities.Orders order, bool isOrderOk, Ticket ticket, string line)
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

        private async Task<string> GetOrderNumber(string tableOpeningFamilyId) 
        {
            QuerySnapshot documentSnapshots = await _db.Collection("orders").WhereEqualTo("tableOpeningFamilyId", tableOpeningFamilyId).GetSnapshotAsync();
            if (documentSnapshots.Documents.Count() == 0) 
            {
                documentSnapshots = await _db.Collection("orders").WhereEqualTo("tableOpeningId", tableOpeningFamilyId).GetSnapshotAsync();
            }
            var order = documentSnapshots.Documents.Select(d => d.ToDictionary().GetObject<Menoo.PrinterService.Business.Entities.Orders>()).ToList();
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

        private async Task SaveOrderAsync(Entities.Orders order)
        {
            string comment = string.Empty;
            var ticket = new Ticket 
            {
                StoreId = order.StoreId,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.ORDER.ToString()
            };
            bool isOrderOk = order?.OrderType != null;
            if (IsTakeAway(order, isOrderOk))
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, -5);
            }
            else
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, 30);
                ticket.TableOpeningFamilyId = order.TableOpeningFamilyId;
            }
            string lines = CreateComments(order);
            await CreateOrderTicket(order, isOrderOk, ticket, lines);
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task SaveOrderAsync(OrdersCancelled order)
        {
            var ticket = new Ticket {
                StoreId = order.Store.Id,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                TicketType = TicketTypeEnum.CANCELLED_ORDER.ToString()
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

        private async Task GenerateOrderQR(Entities.Orders order) 
        {
            
        }
        #endregion
    }
}
