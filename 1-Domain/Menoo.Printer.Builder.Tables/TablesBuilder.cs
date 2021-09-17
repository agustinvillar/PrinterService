using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Enums;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Tables
{
    [Handler]
    public class TablesBuilder : ITicketBuilder
    {
        private readonly PrinterContext _printerContext;

        private readonly EventLog _generalWriter;

        private readonly TableOpeningFamilyRepository _tableOpeningFamilyRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;

        private readonly OrderRepository _orderRepository;

        public TablesBuilder(
            PrinterContext printerContext,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            OrderRepository orderRepository,
            TableOpeningFamilyRepository tableOpeningFamilyRepository) 
        {
            _printerContext = printerContext;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _tableOpeningFamilyRepository = tableOpeningFamilyRepository;
            _orderRepository = orderRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public override string ToString()
        {
            return PrintBuilder.TABLE_BUILDER;
        }

        public async Task BuildAsync(string id, PrintMessage data)
        {
            if (data.Builder != PrintBuilder.TABLE_BUILDER)
            {
                return;
            }
            var tableOpeningFamilyDTO = await _tableOpeningFamilyRepository.GetById<TableOpeningFamily>(data.DocumentId, "tableOpeningFamily");
            if (data.PrintEvent == PrintEvents.TABLE_OPENED)
            {
                await BuildOpenTableOpeningFamilyAsync(id, tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                await BuildCloseTableOpeningFamilyAsync(id, tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.REQUEST_PAYMENT) 
            {
                await BuildRequestPaymentAsync(id, tableOpeningFamilyDTO, data.SubTypeDocument);
            }
        }

        #region private methods
        private async Task BuildCloseTableOpeningFamilyAsync(string id, TableOpeningFamily tableOpeningFamilyDTO)
        {
            var store = await _storeRepository.GetById<Store>(tableOpeningFamilyDTO.StoreId, "stores");
            var sectors = store.GetPrintSettings(PrintEvents.TABLE_CLOSED);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.Where(f => f.AllowPrinting).OrderBy(o => o.Name))
                {
                    var ticket = new Ticket
                    {
                        TicketType = TicketTypeEnum.CLOSE_TABLE.ToString(),
                        StoreId = tableOpeningFamilyDTO.StoreId,
                        PrintBefore = Utils.BeforeAt(tableOpeningFamilyDTO.ClosedAt, 10),
                        Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        Copies = sector.Copies,
                        PrinterName = sector.Printer
                    };
                    StringBuilder orderViewData = new StringBuilder();
                    double total = 0;
                    if (tableOpeningFamilyDTO.TableOpenings.Count() > 0)
                    {
                        foreach (var to in tableOpeningFamilyDTO.TableOpenings)
                        {
                            orderViewData.Append($"<p><b>{to.UserName}</b></p>");
                            foreach (var order in to.Orders)
                            {
                                var orderData = await _orderRepository.GetById<OrderV2>(order.Id, "orders");
                                if (orderData.Status.ToLower() == "cancelado") 
                                {
                                    continue;
                                }
                                foreach (var item in order.Items)
                                {
                                    var quantityLabel = item.Quantity > 1 ? "unidades" : "unidad";
                                    orderViewData.Append($"<p>{Utils.GetTime(order.MadeAt)} {item.Name} x {item.Quantity} {quantityLabel} ${item.PriceToTicket}</p>");
                                    if (item.Options != null)
                                    {
                                        foreach (var option in item.Options)
                                        {
                                            if (option != null)
                                            {
                                                orderViewData.Append($"<p>{option.Name} {option.Price}</p>");
                                            }

                                        }
                                    }
                                }
                            }
                            if (to.CutleryPriceTotal != null && to.CutleryPriceTotal > 0)
                            {
                                orderViewData.Append($"<p>Cubiertos x{to.CulteryPriceQuantity}: ${to.CutleryPriceTotal}</p>");
                            }

                            if (to.ArtisticCutleryTotal != null && to.ArtisticCutleryTotal > 0)
                            {
                                orderViewData.Append($"<p>Cubierto Artistico x{to.ArtisticCutleryQuantity}: ${to.ArtisticCutleryTotal}</p>");
                            }

                            if (to.Tip != null && to.Tip > 0)
                            {
                                orderViewData.Append($"<p>Propina: ${to.Tip}</p>");
                            }

                            if (to.Surcharge != null && to.Surcharge > 0)
                            {
                                orderViewData.Append($"<p>Adicional por servicio: ${to.Surcharge}</p>");
                            }

                            if (to.Discounts != null && to.Discounts.Length > 0)
                            {
                                var discounts = to.Discounts.Where(discount => discount.Type != DiscountTypeEnum.Iva);
                                foreach (var detail in discounts)
                                {
                                    orderViewData.Append($"<p>Descuento {detail.Name}: -${detail.Amount}</p>");
                                }
                            }

                            if (!string.IsNullOrEmpty(to.PayMethod))
                            {
                                orderViewData.Append($"Método de Pago: {to.PayMethod}");
                            }

                            if (to.PagoPorTodos || to.PagoPorElMismo)
                            {
                                orderViewData.Append($"<p>Subtotal: ${to.TotalToTicket(store)}</p>");
                            }

                            if (to.PagoPorElMismo)
                            {
                                orderViewData.Append("<p>Pagó su propia cuenta</p>");
                            }

                            if (to.PagoPorTodos)
                            {
                                orderViewData.Append("<p>Pagó la cuenta de todos.</p>");
                            }

                            if (to.AlguienLePago)
                            {
                                orderViewData.Append("<p>Le pagaron su cuenta.</p>");
                            }
                        }
                        total = tableOpeningFamilyDTO.TotalToTicket(store);
                    }
                    ticket.SetTableClosing(SetTitleForCloseTable(tableOpeningFamilyDTO), tableOpeningFamilyDTO.TableNumberToShow, tableOpeningFamilyDTO.ClosedAt, total.ToString(), orderViewData.ToString());
                    await _ticketRepository.SaveAsync(ticket);
                    await _ticketRepository.SetDocumentHtmlAsync(id, ticket.Data);
                }
            }
        }

        private async Task BuildOpenTableOpeningFamilyAsync(string id, TableOpeningFamily tableOpeningFamilyDTO)
        {
            var store = await _storeRepository.GetById<Store>(tableOpeningFamilyDTO.StoreId, "stores");
            var sectors = store.GetPrintSettings(PrintEvents.TABLE_OPENED);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.Where(f => f.AllowPrinting).OrderBy(o => o.Name))
                {
                    var ticket = new Ticket
                    {
                        TicketType = TicketTypeEnum.OPEN_TABLE.ToString(),
                        PrintBefore = Utils.BeforeAt(tableOpeningFamilyDTO.OpenedAt, 10),
                        StoreId = tableOpeningFamilyDTO.StoreId,
                        StoreName = store.Name,
                        Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                        Copies = sector.Copies,
                        PrinterName = sector.Printer
                    };
                    ticket.SetTableOpening("Apertura de mesa", tableOpeningFamilyDTO.TableNumberToShow, tableOpeningFamilyDTO.OpenedAt);
                    await _ticketRepository.SaveAsync(ticket);
                    await _ticketRepository.SetDocumentHtmlAsync(id, ticket.Data);
                }
            }
        }

        private async Task BuildRequestPaymentAsync(string id, TableOpeningFamily tableOpeningFamilyDTO, string typeRequest)
        {
            var store = await _storeRepository.GetById<Store>(tableOpeningFamilyDTO.StoreId, "stores");
            var sectors = store.GetPrintSettings(PrintEvents.REQUEST_PAYMENT);
            if (sectors.Count > 0)
            {
                foreach (var sector in sectors.Where(f => f.AllowPrinting).OrderBy(o => o.Name))
                {
                    await SaveRequestPayment(id, tableOpeningFamilyDTO, typeRequest, store, sector);
                }
            }
        }

        private async Task SaveRequestPayment(string id, TableOpeningFamily tableOpeningFamilyDTO, string typeRequest, Store store, PrintSettings sector)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            string title = string.Empty;
            
            var ticket = new Ticket
            {
                TicketType = TicketTypeEnum.PAYMENT_REQUEST.ToString(),
                PrintBefore = Utils.BeforeAt(now, 10),
                StoreId = tableOpeningFamilyDTO.StoreId,
                StoreName = store.Name,
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                Copies = sector.Copies,
                PrinterName = sector.Printer
            };

            StringBuilder orderViewData = new StringBuilder();
            if (tableOpeningFamilyDTO.TableOpenings.Count() > 0)
            {
                foreach (var to in tableOpeningFamilyDTO.TableOpenings)
                {
                    if (to.PayWithPOS)
                    {
                        title = "Solicitud de pago POS";
                    }
                    else
                    {
                        title = "Solicitud de pago efectivo";
                    }
                    orderViewData.Append($"<p>Cliente: {to.UserName}</p>");
                    foreach (var order in to.Orders)
                    {
                        var orderData = await _orderRepository.GetById<OrderV2>(order.Id, "orders");
                        if (orderData.Status.ToLower() == "cancelado")
                        {
                            continue;
                        }
                        foreach (var item in order.Items)
                        {
                            var quantityLabel = item.Quantity > 1 ? "unidades" : "unidad";
                            orderViewData.Append($"<p>{Utils.GetTime(order.MadeAt)} {item.Name} x {item.Quantity} {quantityLabel} ${item.PriceToTicket}</p>");
                            if (item.Options != null)
                            {
                                foreach (var option in item.Options)
                                {
                                    if (option != null)
                                    {
                                        orderViewData.Append($"<p>{option.Name} {option.Price}</p>");
                                    }
                                }
                            }
                        }
                    }
                    if (to.CutleryPriceTotal != null && to.CutleryPriceTotal > 0)
                    {
                        orderViewData.Append($"<p>Cubiertos x{to.CulteryPriceQuantity}: ${to.CutleryPriceTotal}</p>");
                    }

                    if (to.ArtisticCutleryTotal != null && to.ArtisticCutleryTotal > 0)
                    {
                        orderViewData.Append($"<p>Cubierto Artistico x{to.ArtisticCutleryQuantity}: ${to.ArtisticCutleryTotal}</p>");
                    }

                    if (to.Tip != null && to.Tip > 0)
                    {
                        orderViewData.Append($"<p>Propina: ${to.Tip}</p>");
                    }

                    if (to.Surcharge != null && to.Surcharge > 0)
                    {
                        orderViewData.Append($"<p>Adicional por servicio: ${to.Surcharge}</p>");
                    }

                    if (to.Discounts != null && to.Discounts.Length > 0)
                    {
                        var discounts = to.Discounts.Where(discount => discount.Type != DiscountTypeEnum.Iva);
                        foreach (var detail in discounts)
                        {
                            orderViewData.Append($"<p>Descuento {detail.Name}: -${detail.Amount}</p>");
                        }
                    }

                    if (to.PagoPorTodos || to.PagoPorElMismo)
                    {
                        orderViewData.Append($"<p>Subtotal: ${to.TotalToTicket(store)}</p>");
                    }
                }
                double total = tableOpeningFamilyDTO.TotalToTicket(store);
                ticket.SetRequestPayment(title, tableOpeningFamilyDTO.TableNumberToShow, DateTime.Now.ToString("dd/MM/yyyy HH:mm"), total.ToString(), orderViewData.ToString());
                await _ticketRepository.SaveAsync(ticket);
                await _ticketRepository.SetDocumentHtmlAsync(id, ticket.Data);
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
        #endregion
    }
}
