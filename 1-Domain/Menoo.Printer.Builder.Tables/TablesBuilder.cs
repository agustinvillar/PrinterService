﻿using Google.Cloud.Firestore;
using Menoo.Printer.Builder.Tables.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
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
        private readonly FirestoreDb _firestoreDb;

        private readonly EventLog _generalWriter;

        private readonly TableOpeningFamilyRepository _tableOpeningFamilyRepository;

        private readonly StoreRepository _storeRepository;

        private readonly TicketRepository _ticketRepository;


        public TablesBuilder(
            FirestoreDb firestoreDb,
            StoreRepository storeRepository,
            TicketRepository ticketRepository,
            TableOpeningFamilyRepository tableOpeningFamilyRepository) 
        {
            _firestoreDb = firestoreDb;
            _storeRepository = storeRepository;
            _ticketRepository = ticketRepository;
            _tableOpeningFamilyRepository = tableOpeningFamilyRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public async Task BuildAsync(PrintMessage data)
        {
            if (data.Builder != PrintBuilder.TABLE_BUILDER)
            {
                return;
            }
            var tableOpeningFamilyDTO = await _tableOpeningFamilyRepository.GetById<TableOpeningFamily>(data.DocumentId, "tableOpeningFamily");
            if (data.PrintEvent == PrintEvents.TABLE_OPENED)
            {
                await BuildOpenTableOpeningFamilyAsync(tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                await BuildCloseTableOpeningFamilyAsync(tableOpeningFamilyDTO);
            }
        }

        #region private methods
        private async Task BuildOpenTableOpeningFamilyAsync(TableOpeningFamily tableOpeningFamilyDTO)
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
                }
            }
        }

        private async Task BuildCloseTableOpeningFamilyAsync(TableOpeningFamily tableOpeningFamilyDTO)
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
                    StringBuilder orderData = new StringBuilder();
                    double total = 0;
                    if (tableOpeningFamilyDTO.TableOpenings.Count() > 0)
                    {
                        foreach (var to in tableOpeningFamilyDTO.TableOpenings)
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
                                var discounts = to.Discounts.Where(discount => discount.Type != DiscountTypeEnum.Iva);
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
                        total = tableOpeningFamilyDTO.TotalToTicket(store);
                    }
                    ticket.SetTableClosing(SetTitleForCloseTable(tableOpeningFamilyDTO), tableOpeningFamilyDTO.TableNumberToShow, tableOpeningFamilyDTO.ClosedAt, total.ToString(), orderData.ToString());
                    await _ticketRepository.SaveAsync(ticket);
                }
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
