using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Menoo.PrinterService.Business.Entities.Ticket;

namespace Menoo.PrinterService.Business.Orders
{
    /// <summary>
    /// Maneja los eventos de la colección orders
    /// </summary>
    public class OrdersManager
    {
        private const string MESAS = "MESAS";

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
                Utils.LogError(ex.Message);
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
                var order = document.ConvertTo<Entities.Orders>();
                var storeData = Utils.GetStores(_db, order.Store.StoreId).GetAwaiter().GetResult();
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
                SaveOrderAsync(order).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }
        #endregion

        #region private methods
        private static List<string> CreateComments(Entities.Orders order)
        {
            var lines = new List<string>();
            if (order.Items == null) return lines;

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
            return lines;
        }

        private static List<string> CreateComments(OrderCancelled order)
        {
            var lines = new List<string>();
            if (order.Items == null)
            {
                return lines;
            }

            foreach (var item in order.Items)
            {
                if (item != null)
                {
                    lines.Add($"<b>--{item.Name}</b> x {item.Quantity}");
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
            return lines;
        }

        private static string CreateHtmlFromLines(List<string> lines)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }
            string items = lines.Aggregate(string.Empty, (current, line) => current + ($"<p>{line}</p>"));
            return items;
        }

        private static void CreateOrderTicket(OrderCancelled order, Ticket ticket, string line, string orderType)
        {
            string table = "";
            ticket.TicketType = TicketTypeEnum.ORDER.ToString();
            string title;
            switch (orderType.ToUpper())
            {
                case "RESERVA":
                    title = $"Orden #{order.OrderNumber} (RESERVA) cancelada";
                    break;

                case "TAKEAWAY":
                    title = $"Orden #{order.OrderNumber} (TAKE AWAY) cancelada";
                    break;

                default:
                    title = $"Orden #{order.OrderNumber} cancelada";
                    table = $"Servir en mesa: {order.Address}";
                    break;
            }
            string client = $"Cliente: {order.UserName}";
            ticket.Data += $"<h1>{title}</h1><br/><h3>{client}{line}{table}</h3></body></html>";
        }

        private static bool IsTakeAway(Entities.Orders order, bool orderOk)
        {
            return orderOk && order.IsTakeAway;
        }

        private async Task CreateOrderTicket(Entities.Orders order, bool isOrderOk, Ticket ticket, string line)
        {
            ticket.TicketType = TicketTypeEnum.ORDER.ToString();
            string title, client;

            if (isOrderOk && order.OrderType.ToUpper().Trim() == MESAS)
            {
                title = "Nueva orden de mesa";
                client = $"Cliente: {order.UserName}";
                var table = $"Servir en mesa: {order.Address}";
                ticket.Data += $"<h1>{title}</h1><h3>{client}{line}{table}</h3></body></html>";
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == RESERVA)
            {
                title = "Nueva orden de reserva";
                client = $"Cliente: {order.UserName}";
                ticket.Data += $"<h1>{title}</h1><h3>{client}{line}</h3></body></html>";
            }
            else if (isOrderOk && order.OrderType.ToUpper().Trim() == TAKEAWAY)
            {
                var payment = await GetPayment(order.TableOpeningFamilyId, TAKEAWAY);
                title = "Nuevo Take Away";
                client = $"Cliente: {order.UserName}";
                var time = $"Hora de retiro: {order.TakeAwayHour}";
                var paymentInfo = string.Empty;
                if (payment != null)
                {
                    paymentInfo += $"<h3>Método de Pago: {payment.PaymentMethod}</h3>";
                    paymentInfo += $"<h1>--------------------------------------------------</h1>";
                    paymentInfo += $"<h1>Recuerde ACEPTAR el pedido.</h1>";
                    if (order.Store.PaymentProvider == Store.ProviderEnum.MercadoPago) paymentInfo += $"<h1>Pedido YA PAGO.</h1>";
                    paymentInfo += $"<h1>--------------------------------------------------</h1>";
                    paymentInfo += $"<h1>Total: ${payment.TotalToPayTicket}</h1>";
                }
                ticket.Data += $"<h1>{title}</h1><h3>{client}{line}{time}</h3>{paymentInfo}</body></html>";
            }
        }

        private async Task<Payment> GetPayment(string id, string type)
        {
            var filter = type == MESAS ? "tableOpening.id" : "taOpening.id";
            var documentSnapshots = await _db.Collection("payments").WhereEqualTo(filter, id).GetSnapshotAsync();
            var payments = documentSnapshots.Documents.Select(d => d.ConvertTo<Payment>()).ToList();
            return payments.FirstOrDefault();
        }

        private async Task SaveOrderAsync(Entities.Orders order)
        {
            //if (!AllowPrint(order.Store)) 
            //{
            //    return;
            //}
           
            var comment = string.Empty;
            var ticket = Utils.CreateInstanceOfTicket();
            var lines = CreateComments(order);

            var isOrderOk = order?.OrderType != null;
            if (IsTakeAway(order, isOrderOk))
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, -5);
            }
            else
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, 30);
                ticket.TableOpeningFamilyId = order.TableOpeningFamilyId;
            }
            string line = CreateHtmlFromLines(lines);
            await CreateOrderTicket(order, isOrderOk, ticket, line);
            ticket.StoreId = order.StoreId;
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task SaveOrderAsync(OrderCancelled order)
        {
            var ticket = Utils.CreateInstanceOfTicket();
            var lines = CreateComments(order);
            if (order.OrderType.ToLower() == "takeaway")
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, -5);
            }
            else
            {
                ticket.PrintBefore = Utils.BeforeAt(order.OrderDate, 30);
            }
            var line = CreateHtmlFromLines(lines);
            CreateOrderTicket(order, ticket, line, order.OrderType);
            ticket.StoreId = order.Store.Id;
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            await Utils.SaveTicketAsync(_db, ticket);
        }
        #endregion
    }
}
