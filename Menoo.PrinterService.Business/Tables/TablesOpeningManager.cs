using Google.Cloud.Firestore;
using Menoo.PrinterService.Business.Core;
using Menoo.PrinterService.Business.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            _db.Collection("tableOpeningFamily")
               .WhereEqualTo("closed", true)
               .Listen(OnClose);

            _db.Collection("tableOpeningFamily")
                .WhereEqualTo("closed", false)
                .Listen(OnRequestPayment);

            _db.Collection("posConfirmations")
                .Limit(1)
                .Listen(OnPosRequest);

            _db.Collection("cashConfirmations")
                 .Limit(1)
                 .Listen(OnCashRequest);
        }

        #region events
        private void OnClose(QuerySnapshot snapshot)
        {
            try
            {
                var document = snapshot.Documents.OrderByDescending(o => o.UpdateTime).FirstOrDefault();
                var tableOpeningFamily = document.ToDictionary().GetObject<TableOpeningFamily>();
                if (tableOpeningFamily.Closed && !tableOpeningFamily.ClosedPrinted)
                {
                    SaveCloseTableOpeningFamily(document.Id, tableOpeningFamily).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }

        private void OnOpenFamily(QuerySnapshot snapshot)
        {
            try
            {
                var document = snapshot.Documents.Single();
                var tableOpeningFamily = document.ToDictionary().GetObject<TableOpeningFamily>();
                if (!tableOpeningFamily.Closed && !tableOpeningFamily.OpenPrinted)
                {
                    SaveOpenTableOpeningFamily(document.Id, tableOpeningFamily).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Utils.LogError(ex.ToString());
            }
        }

        private void OnRequestPayment(QuerySnapshot snapshot)
        {
            try
            {
                var requestPayment = snapshot.Documents.OrderByDescending(o => o.UpdateTime).FirstOrDefault();
                var document = requestPayment.ToDictionary();
                bool isClosed = bool.Parse(document["closed"].ToString());
                var tableOpenings = ((IEnumerable)document["tableOpenings"]).Cast<dynamic>();
                if (!isClosed && ContainsPayWithPOSProperty(tableOpenings))
                {
                    bool payWithPos = document.GetObject<TableOpeningFamily>().TableOpenings.Any(f => f.PayWithPOS);
                    if (payWithPos)
                    {
                        _db.Collection("posConfirmations").AddAsync(document).GetAwaiter().GetResult();
                    }
                    else
                    {
                        _db.Collection("cashConfirmations").AddAsync(document).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError(e.ToString());
            }
        }

        private void OnPosRequest(QuerySnapshot snapshot)
        {
            try
            {
                if (snapshot.Documents.Count == 0) 
                {
                    return;
                }
                var document = snapshot.Documents.Single();
                var tableOpeningFamily = document.ToDictionary().GetObject<TableOpeningFamily>();
                SaveRequestPayment(tableOpeningFamily, "Solicitud de pago POS").GetAwaiter().GetResult();
                _db.Collection("posConfirmations").Document(document.Id).DeleteAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Utils.LogError(e.ToString());
            }
        }

        private void OnCashRequest(QuerySnapshot snapshot)
        {
            try
            {
                if (snapshot.Documents.Count == 0)
                {
                    return;
                }
                var document = snapshot.Documents.Single();
                var tableOpeningFamily = document.ToDictionary().GetObject<TableOpeningFamily>();
                SaveRequestPayment(tableOpeningFamily, "Solicitud de pago efectivo").GetAwaiter().GetResult();
                _db.Collection("cashConfirmations").Document(document.Id).DeleteAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Utils.LogError(e.ToString());
            }
        }

        #endregion

        #region private methods
        private bool ContainsPayWithPOSProperty(IEnumerable<dynamic> tableOpenings)
        {
            int count = 0;
            foreach (var element in tableOpenings)
            {
                var item = (Dictionary<string, object>)element;
                if (item.ContainsKey("payWithPOS"))
                {
                    count++;
                }
            }
            return count > 0;
        }

        private async Task SaveCloseTableOpeningFamily(string documentId, TableOpeningFamily tableOpeningFamily)
        {
            //bool tableOpeningFamilyAlreadyExists = await TableOpeningFamilyAlreadyExists(tableOpeningFamily.Id);
            //if (tableOpeningFamilyAlreadyExists)
            //{
            //    return;
            //}
            var store = await Utils.GetStores(_db, tableOpeningFamily.StoreId);
            var sectors = store.GetPrintSettings(PrintEvents.TABLE_CLOSED);
            if (sectors.Count > 0)
            {
                SetTableOpeningFamilyPrintedAsync(tableOpeningFamily.Id, PrintedEvent.CLOSING).GetAwaiter().GetResult();
                foreach (var sector in sectors)
                {
                    if (sector.AllowPrinting && tableOpeningFamily.Closed)
                    {
                        var ticket = new Ticket
                        {
                            TicketType = TicketTypeEnum.CLOSE_TABLE.ToString(),
                            StoreId = tableOpeningFamily.StoreId,
                            PrintBefore = Utils.BeforeAt(tableOpeningFamily.ClosedAt, 10),
                            Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                            Copies = sector.Copies,
                            PrinterName = sector.Printer
                        };
                        StringBuilder orderData = new StringBuilder();
                        double total = 0;
                        if (tableOpeningFamily.TableOpenings.Count() > 0)
                        {
                            foreach (var to in tableOpeningFamily.TableOpenings)
                            {
                                orderData.Append($"<p>{to.UserName}</p>");
                                foreach (var order in to.Orders)
                                {
                                    foreach (var item in order.Items)
                                    {
                                        var quantityLabel = item.Quantity > 1 ? "unidades" : "unidad";
                                        orderData.Append($"<p>{Utils.GetTime(order.MadeAt)} {item.Name} x {item.Quantity} {quantityLabel} ${item.PriceToTicket}</p>");
                                        if (item.Options != null)
                                        {
                                            foreach (var option in item.Options)
                                            {
                                                if (option != null)
                                                {
                                                    orderData.Append($"<p>{option.Name} {option.Price}</p>");
                                                }

                                            }
                                        }
                                    }
                                }
                                if (to.CutleryPriceTotal != null && to.CutleryPriceTotal > 0)
                                {
                                    orderData.Append($"<p>Cubiertos x{to.CulteryPriceQuantity}: ${to.CutleryPriceTotal}</p>");
                                }

                                if (to.ArtisticCutleryTotal != null && to.ArtisticCutleryTotal > 0)
                                {
                                    orderData.Append($"<p>Cubierto Artistico x{to.ArtisticCutleryQuantity}: ${to.ArtisticCutleryTotal}</p>");
                                }

                                if (to.Tip != null && to.Tip > 0)
                                {
                                    orderData.Append($"<p>Propina: ${to.Tip}</p>");
                                }

                                if (to.Surcharge != null && to.Surcharge > 0)
                                {
                                    orderData.Append($"<p>Adicional por servicio: ${to.Surcharge}</p>");
                                }

                                if (to.Discounts != null && to.Discounts.Length > 0)
                                {
                                    var discounts = to.Discounts.Where(discount => discount.Type != DiscountType.Iva);
                                    foreach (var detail in discounts)
                                    {
                                        orderData.Append($"<p>Descuento {detail.Name}: -${detail.Amount}</p>");
                                    }
                                }

                                if (!string.IsNullOrEmpty(to.PayMethod))
                                {
                                    orderData.Append($"Método de Pago: {to.PayMethod}");
                                }

                                if (to.PagoPorTodos || to.PagoPorElMismo)
                                {
                                    orderData.Append($"<p>Subtotal: ${to.TotalToTicket(store)}</p>");
                                }

                                if (to.PagoPorElMismo)
                                {
                                    orderData.Append("<p>Pagó su propia cuenta</p>");
                                }

                                if (to.PagoPorTodos)
                                {
                                    orderData.Append("<p>Pagó la cuenta de todos.</p>");
                                }

                                if (to.AlguienLePago)
                                {
                                    orderData.Append("<p>Le pagaron su cuenta.</p>");
                                }
                            }
                            total = tableOpeningFamily.TotalToTicket(store);
                        }
                        ticket.SetTableClosing(SetTitleForCloseTable(tableOpeningFamily), tableOpeningFamily.TableNumberToShow, tableOpeningFamily.ClosedAt, total.ToString(), orderData.ToString());
                        Utils.SaveTicketAsync(_db, ticket).GetAwaiter().GetResult();
                    }
                }
            }
        }

        private async Task SaveOpenTableOpeningFamily(string documentId, TableOpeningFamily tableOpeningFamily)
        {
            var store = await Utils.GetStores(_db, tableOpeningFamily.StoreId);
            var sectors = store.GetPrintSettings(PrintEvents.TABLE_OPENED);
            if (sectors.Count > 0)
            {
                SetTableOpeningFamilyPrintedAsync(documentId, PrintedEvent.OPENING).GetAwaiter().GetResult();
                foreach (var sector in sectors)
                {
                    if (sector.AllowPrinting)
                    {
                        var ticket = new Ticket
                        {
                            TicketType = TicketTypeEnum.OPEN_TABLE.ToString(),
                            PrintBefore = Utils.BeforeAt(tableOpeningFamily.OpenedAt, 10),
                            StoreId = tableOpeningFamily.StoreId,
                            Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                            Copies = sector.Copies,
                            PrinterName = sector.Printer
                        };
                        ticket.SetTableOpening("Apertura de mesa", tableOpeningFamily.TableNumberToShow, tableOpeningFamily.OpenedAt);
                        await Utils.SaveTicketAsync(_db, ticket);
                    }
                }
            }
        }

        private async Task SaveRequestPayment(TableOpeningFamily tableOpeningFamily, string title)
        {
            bool tableOpeningFamilyAlreadyExists = await TableOpeningFamilyAlreadyExists(tableOpeningFamily.Id);
            if (tableOpeningFamilyAlreadyExists)
            {
                return;
            }
            var store = await Utils.GetStores(_db, tableOpeningFamily.StoreId);
            var sectors = store.GetPrintSettings(PrintEvents.REQUEST_PAYMENT);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors) 
                {
                    var ticket = new Ticket
                    {
                        TicketType = TicketTypeEnum.PAYMENT_REQUEST.ToString(),
                        StoreId = tableOpeningFamily.StoreId,
                        Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        Copies = sector.Copies,
                        PrinterName = sector.Printer,
                        TableOpeningFamilyId = tableOpeningFamily.Id
                    };
                    StringBuilder orderData = new StringBuilder();
                    if (tableOpeningFamily.TableOpenings.Count() > 0)
                    {
                        foreach (var to in tableOpeningFamily.TableOpenings)
                        {
                            orderData.Append($"<p>Cliente: {to.UserName}</p>");
                            foreach (var order in to.Orders)
                            {
                                foreach (var item in order.Items)
                                {
                                    var quantityLabel = item.Quantity > 1 ? "unidades" : "unidad";
                                    orderData.Append($"<p>{Utils.GetTime(order.MadeAt)} {item.Name} x {item.Quantity} {quantityLabel} ${item.PriceToTicket}</p>");
                                    if (item.Options != null)
                                    {
                                        foreach (var option in item.Options)
                                        {
                                            if (option != null)
                                            {
                                                orderData.Append($"<p>{option.Name} {option.Price}</p>");
                                            }

                                        }
                                    }
                                }
                            }
                            if (to.CutleryPriceTotal != null && to.CutleryPriceTotal > 0)
                            {
                                orderData.Append($"<p>Cubiertos x{to.CulteryPriceQuantity}: ${to.CutleryPriceTotal}</p>");
                            }

                            if (to.ArtisticCutleryTotal != null && to.ArtisticCutleryTotal > 0)
                            {
                                orderData.Append($"<p>Cubierto Artistico x{to.ArtisticCutleryQuantity}: ${to.ArtisticCutleryTotal}</p>");
                            }

                            if (to.Tip != null && to.Tip > 0)
                            {
                                orderData.Append($"<p>Propina: ${to.Tip}</p>");
                            }

                            if (to.Surcharge != null && to.Surcharge > 0)
                            {
                                orderData.Append($"<p>Adicional por servicio: ${to.Surcharge}</p>");
                            }

                            if (to.Discounts != null && to.Discounts.Length > 0)
                            {
                                var discounts = to.Discounts.Where(discount => discount.Type != DiscountType.Iva);
                                foreach (var detail in discounts)
                                {
                                    orderData.Append($"<p>Descuento {detail.Name}: -${detail.Amount}</p>");
                                }
                            }

                            if (to.PagoPorTodos || to.PagoPorElMismo)
                            {
                                orderData.Append($"<p>Subtotal: ${to.TotalToTicket(store)}</p>");
                            }
                        }
                        double total = tableOpeningFamily.TotalToTicket(store);
                        ticket.SetRequestPayment(title, tableOpeningFamily.TableNumberToShow, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), total.ToString(), orderData.ToString());
                        Utils.SaveTicketAsync(_db, ticket).GetAwaiter().GetResult();
                    }
                }
            }
        }

        private async Task<WriteResult> SetTableOpeningFamilyPrintedAsync(string doc, PrintedEvent printEvent)
        {
            if (printEvent == PrintedEvent.CLOSING)
            {
                return await _db.Collection("tableOpeningFamily").Document(doc).UpdateAsync("closedPrinted", true);
            }
            else if (printEvent == PrintedEvent.OPENING)
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
