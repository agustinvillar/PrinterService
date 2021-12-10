using Menoo.Printer.Builder.Orders.Repository;
using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Database.SqlServer.PrinterSchema;
using Menoo.PrinterService.Infraestructure.Extensions;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using Menoo.PrinterService.Infraestructure.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    var date = Convert.ToDateTime(tableOpeningFamilyDTO.OpenedAt);
                    var viewData = new Dictionary<string, string>() 
                    {
                        { "title", "APERTURA DE MESA" },
                        { "tableNumber", tableOpeningFamilyDTO.TableNumberToShow },
                        { "openedAt", tableOpeningFamilyDTO.OpenedAt }
                    };
                    var formatService = FormaterFactory.Resolve(sector.IsHTML, viewData, "Ticket_Table_Opened");
                    await formatService.PrintAsync();
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
