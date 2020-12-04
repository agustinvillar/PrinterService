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
    public class TablesOpeningManager
    {
        private readonly FirestoreDb _db;

        public TablesOpeningManager(FirestoreDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Escucha los eventos disparados en la colección tableOpeningFamily.
        /// </summary>
        public void Listen()
        {
            _db.Collection("tableOpeningFamily")
                  .OrderByDescending("openedAtNumber")
                  .Limit(1)
                  .Listen(OnOpenFamily);

            var date = (DateTime.UtcNow.AddHours(-15) - new DateTime(1970, 1, 1)).TotalSeconds;

            _db.Collection("tableOpeningFamily")
               .WhereEqualTo("closed", true)
               .WhereGreaterThanOrEqualTo("openedAtNumber", date)
               .Listen(OnOpen);
        }

        #region events
        private void OnOpen(QuerySnapshot snapshot)
        {
            try
            {
                var docs = snapshot.Documents.Select(d =>
                {
                    var dic = d.ToDictionary();
                    var toFamily = d.ConvertTo<TableOpeningFamily>();
                    toFamily.TotalToPay = dic.ContainsKey("totalToPay") && dic["totalToPay"] != null ? double.Parse(dic["totalToPay"].ToString()) : 0;
                    return toFamily;
                });
                foreach (var tableOpeningFamily in docs)
                {
                    if (tableOpeningFamily.Closed && !tableOpeningFamily.ClosedPrinter)
                    {
                        SetTableOpeningFamilyPrintedAsync(tableOpeningFamily.Id, TableOpeningFamily.PrintedEvent.CLOSING).GetAwaiter().GetResult();
                        SaveCloseTableOpeningFamily(tableOpeningFamily).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }

        private void OnOpenFamily(QuerySnapshot snapshot)
        {
            try
            {
                var document = snapshot.Documents.Single();
                var tableOpeningFamily = document.ConvertTo<TableOpeningFamily>();

                if (!tableOpeningFamily.Closed && !tableOpeningFamily.OpenPrinted)
                {
                    SetTableOpeningFamilyPrintedAsync(document.Id, TableOpeningFamily.PrintedEvent.OPENING).GetAwaiter().GetResult();
                    SaveOpenTableOpeningFamily(tableOpeningFamily).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.Message);
            }
        }
        #endregion

        #region private methods
        private async Task SaveCloseTableOpeningFamily(TableOpeningFamily tableOpeningFamily)
        {
            bool tableOpeningFamilyAlreadyExists = await TableOpeningFamilyAlreadyExists(tableOpeningFamily.Id);
            if (tableOpeningFamilyAlreadyExists)
            {
                return;
            }
            var store = await Utils.GetStores(_db, tableOpeningFamily.StoreId);
            if (!store.AllowPrint(PrintEvents.TABLE_CLOSED)) 
            {
                return;
            }
            var ticket = Utils.CreateInstanceOfTicket();
            ticket.TicketType = TicketTypeEnum.CLOSE_TABLE.ToString();

            if (tableOpeningFamily.Closed)
            {
                var title = SetTitleForCloseTable(tableOpeningFamily);
                var tableNumber = $"Número de mesa: {tableOpeningFamily.TableNumberToShow}";
                var orden = "<b>Pedidos</b>";
                orden += "<p><b>------------------------------------------------------</b></p>";
                foreach (var to in tableOpeningFamily.TableOpenings)
                {
                    orden += $"<p>{to.UserName}</p>";
                    foreach (var order in to.Orders)
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
                    if (to.CutleryPriceTotal != null && to.CutleryPriceTotal > 0) orden += $"<p>Cubiertos x{to.CulteryPriceQuantity}: ${to.CutleryPriceTotal}</p>";
                    if (to.ArtisticCutleryTotal != null && to.ArtisticCutleryTotal > 0) orden += $"<p>Cubierto Artistico x{to.ArtisticCutleryQuantity}: ${to.ArtisticCutleryTotal}</p>";
                    if (to.Tip != null && to.Tip > 0) orden += $"<p>Propina: ${to.Tip}</p>";
                    if (to.Surcharge != null && to.Surcharge > 0) orden += $"<p>Adicional por servicio: ${to.Surcharge}</p>";
                    if (to.Discounts != null)
                        orden = to.Discounts.Where(discount => discount.Type != TableOpening.Discount.DiscountType.Iva)
                            .Aggregate(orden, (current, discount) => current + ($"<p>Descuento {discount.Name}: -${discount.Amount}</p>"));

                    if (!string.IsNullOrEmpty(to.PayMethod)) orden += $"Metodo de Pago: {to.PayMethod}";
                    if (to.PagoPorTodos || to.PagoPorElMismo)
                        orden += $"<p>Subtotal: ${to.TotalToTicket(store)}</p>";
                    if (to.PagoPorElMismo) orden += "<p>Pagó su propia cuenta</p>";
                    if (to.PagoPorTodos) orden += "<p>Pagó la cuenta de todos.</p>";
                    if (to.AlguienLePago) orden += "<p>Le pagaron su cuenta.</p>";
                    orden += "<p><b>------------------------------------------------------</b></p>";
                }

                orden += $"<h1>TOTAL: ${tableOpeningFamily.TotalToTicket(store)}</h1>";
                var date = $"Fecha: {tableOpeningFamily.ClosedAt}";
                ticket.Data += $"<h1>{title}</h1><h3><p>{tableNumber}</p><p>{date}</p><p>{orden}</p></h3></body></html>";
            }
            ticket.StoreId = tableOpeningFamily.StoreId;
            ticket.TableOpeningFamilyId = tableOpeningFamily.Id;
            ticket.PrintBefore = Utils.BeforeAt(tableOpeningFamily.ClosedAt, 10);
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            Utils.SaveTicketAsync(_db, ticket).GetAwaiter().GetResult();
        }

        private async Task SaveOpenTableOpeningFamily(TableOpeningFamily tableOpeningFamily)
        {
            var store = await Utils.GetStores(_db, tableOpeningFamily.StoreId);
            if (!store.AllowPrint(PrintEvents.TABLE_OPENED))
            {
                return;
            }
            var ticket = Utils.CreateInstanceOfTicket();
            ticket.TicketType = TicketTypeEnum.OPEN_TABLE.ToString();

            const string title = "Apertura de mesa";
            var tableNumber = $"Número de mesa: {tableOpeningFamily.TableNumberToShow}";
            var date = $"Fecha: {tableOpeningFamily.OpenedAt}";

            ticket.Data += $"<h1>{title}</h1><h3><p>{tableNumber}</p><p>{date}</p></h3></body></html>";
            ticket.PrintBefore = Utils.BeforeAt(tableOpeningFamily.OpenedAt, 10);
            ticket.StoreId = tableOpeningFamily.StoreId;
            ticket.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            ticket.TableOpeningFamilyId = tableOpeningFamily.Id;
            await Utils.SaveTicketAsync(_db, ticket);
        }

        private async Task<WriteResult> SetTableOpeningFamilyPrintedAsync(string doc, TableOpeningFamily.PrintedEvent printEvent)
        {
            if (printEvent == TableOpeningFamily.PrintedEvent.CLOSING)
            {
                return await _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("closedPrinted", true);
            }
            else if (printEvent == TableOpeningFamily.PrintedEvent.OPENING)
            {
                return await _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("openPrinted", true);
            }
            else
            {
                throw new Exception("No se actualizo el estado impreso de la mesa.");
            }
        }

        private string SetTitleForCloseTable(TableOpeningFamily tableOpening)
        {
            string title;
            if (tableOpening.Pending.GetValueOrDefault())
            {
                title = "Mesa abandonada";
            }
            else
            {
                title = "Mesa cerrada";
            }
            return title;
        }

        private async Task<bool> TableOpeningFamilyAlreadyExists(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var query = await _db.Collection("print").WhereEqualTo("tableOpeningFamilyId", id).GetSnapshotAsync();
            return query.Documents.Count == 1;
        }
        #endregion
    }
}
