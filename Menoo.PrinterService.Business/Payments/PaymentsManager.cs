using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Menoo.PrinterService.Business.Entities.Ticket;

namespace Menoo.PrinterService.Business.Tables
{
    /// <summary>
    /// Maneja eventos relacionados a apertura y cierres de mesa.
    /// </summary>
    public class PaymentsManager
    {
        private readonly FirestoreDb _db;

        public PaymentsManager(FirestoreDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Escucha los eventos disparados en la colección tableOpeningFamily.
        /// </summary>
        public void Listen()
        {
            _db.Collection("payments")
               .Listen(OnRequest);
        }

        #region events
        private void OnRequest(QuerySnapshot snapshot)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                var requestPayment = snapshot.Documents.OrderByDescending(o => o.CreateTime).FirstOrDefault(f => f.CreateTime.GetValueOrDefault().ToDateTime().ToString("yyyy-MM-dd") == now);
                if (requestPayment == null)
                {
                    return;
                }
                var document = requestPayment.ToDictionary();
                if (document.ContainsKey("printed"))
                {
                    return;
                }
                if (document.ContainsKey("taOpening") || document.ContainsKey("tableOpening"))
                {
                    Utils.SetOrderPrintedAsync(_db, "payments", requestPayment.Id).GetAwaiter().GetResult();
                    TableOpeningV2 tableOpeningDocument = null;
                    if (document.ContainsKey("taOpening"))
                    {
                        tableOpeningDocument = document["taOpening"].GetObject<TableOpeningV2>();
                    }
                    else if (document.ContainsKey("tableOpening"))
                    {
                        tableOpeningDocument = document["tableOpening"].GetObject<TableOpeningV2>();
                    }
                    SaveCloseTableOpeningFamily(tableOpeningDocument).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }
        #endregion

        #region private methods
        private async Task<TableOpeningFamily> GetDocumentFromTableOpeningFamily(string id)
        {
            var result = await _db.Collection("tableOpeningFamily").Document(id).GetSnapshotAsync();
            var document = result.ConvertTo<TableOpeningFamily>();
            return document;
        }

        private async Task SaveCloseTableOpeningFamily(TableOpeningV2 tableOpening)
        {
            var store = await Utils.GetStores(_db, tableOpening.StoreId);
            if (!store.AllowPrint())
            {
                return;
            }
            var ticket = Utils.CreateInstanceOfTicket();
            ticket.TicketType = TicketTypeEnum.CLOSE_TABLE.ToString();

            string title = "Solicitud de pago (EFECTIVO O POS)";
            var tableNumber = $"Número de mesa: {tableOpening.TableNumberToShow}";
            var orden = "<b>Pedidos</b>";
            orden += "<p><b>------------------------------------------------------</b></p>";

            orden += $"<p>{tableOpening.User.Name}</p>";
            foreach (var order in tableOpening.Orders)
            {
                foreach (var item in order.Items)
                {
                    var quantityLabel = item.Quantity > 1 ? "unidades" : "unidad";
                    orden += $"<p>{Utils.GetTime(order.MadeAt)} {item.Name} x {item.Quantity} {quantityLabel} ${item.PriceToTicket}</p>";
                    if (item.Options != null)
                        foreach (var option in item.Options)
                            if (option != null) orden += $"<p>{option.Name} {option.Price}</p>";
                }
            }
            if (tableOpening.CutleryPriceTotal != null && tableOpening.CutleryPriceTotal > 0) orden += $"<p>Cubiertos x{tableOpening.CulteryPriceQuantity}: ${tableOpening.CutleryPriceTotal}</p>";
            if (tableOpening.ArtisticCutleryTotal != null && tableOpening.ArtisticCutleryTotal > 0) orden += $"<p>Cubierto Artistico x{tableOpening.ArtisticCutleryQuantity}: ${tableOpening.ArtisticCutleryTotal}</p>";
            if (tableOpening.Tip != null && tableOpening.Tip > 0) orden += $"<p>Propina: ${tableOpening.Tip}</p>";
            if (tableOpening.Surcharge != null && tableOpening.Surcharge > 0) orden += $"<p>Adicional por servicio: ${tableOpening.Surcharge}</p>";
            if (tableOpening.Discounts != null)
                orden = tableOpening.Discounts.Where(discount => discount.Type != TableOpening.Discount.DiscountType.Iva)
                    .Aggregate(orden, (current, discount) => current + ($"<p>Descuento {discount.Name}: -${discount.Amount}</p>"));

            if (!string.IsNullOrEmpty(tableOpening.PayMethod)) orden += $"Metodo de Pago: {tableOpening.PayMethod}";
            if (tableOpening.PagoPorTodos || tableOpening.PagoPorElMismo)
                orden += $"<p>Subtotal: ${tableOpening.TotalToTicket(store)}</p>";
            if (tableOpening.PagoPorElMismo) orden += "<p>Pagó su propia cuenta</p>";
            if (tableOpening.PagoPorTodos) orden += "<p>Pagó la cuenta de todos.</p>";
            if (tableOpening.AlguienLePago) orden += "<p>Le pagaron su cuenta.</p>";
            orden += "<p><b>------------------------------------------------------</b></p>";


            orden += $"<h1>TOTAL: ${tableOpening.TotalToTicket(store)}</h1>";
            var date = $"Fecha: {tableOpening.CloseAt}";
            ticket.Data += $"<h1>{title}</h1><h3><p>{tableNumber}</p><p>{date}</p><p>{orden}</p></h3></body></html>";

            ticket.StoreId = tableOpening.StoreId;
            ticket.PrintBefore = Utils.BeforeAt(tableOpening.CloseAt, 10);
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            Utils.SaveTicketAsync(_db, ticket).GetAwaiter().GetResult();
        }

        #endregion
    }
}
