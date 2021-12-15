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

        private readonly OrderRepository _orderRepository;

        public TablesBuilder(
            PrinterContext printerContext,
            StoreRepository storeRepository,
            OrderRepository orderRepository,
            TableOpeningFamilyRepository tableOpeningFamilyRepository) 
        {
            _printerContext = printerContext;
            _storeRepository = storeRepository;
            _tableOpeningFamilyRepository = tableOpeningFamilyRepository;
            _orderRepository = orderRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public override string ToString()
        {
            return PrintBuilder.TABLE_BUILDER;
        }

        public async Task<Tuple<string, string, Store>> BuildAsync(string id, PrintMessage data)
        {
            if (data.Builder != PrintBuilder.TABLE_BUILDER)
            {
                return null;
            }
            string ticket = "";
            var tableOpeningFamilyDTO = await _tableOpeningFamilyRepository.GetById<TableOpeningFamily>(data.DocumentId, "tableOpeningFamily");
            string printBefore = Utils.BeforeAt(tableOpeningFamilyDTO.ClosedAt, 10);
            var store = await _storeRepository.GetById<Store>(tableOpeningFamilyDTO.StoreId, "stores");
            if (data.PrintEvent == PrintEvents.TABLE_OPENED)
            {
                ticket = await BuildOpenTableOpeningFamilyAsync(id, tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                ticket = await BuildCloseTableOpeningFamilyAsync(id, tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.REQUEST_PAYMENT) 
            {
                ticket = await BuildRequestPaymentAsync(id, tableOpeningFamilyDTO, data.SubTypeDocument);
            }
            return new Tuple<string, string, Store>(ticket, printBefore, store);
        }

        #region private methods
        private async Task<string> BuildCloseTableOpeningFamilyAsync(string id, TableOpeningFamily tableOpeningFamilyDTO)
        {
            return string.Empty;
        }

        private async Task<string> BuildOpenTableOpeningFamilyAsync(string id, TableOpeningFamily tableOpeningFamilyDTO)
        {
            return string.Empty;
        }

        private async Task<string> BuildRequestPaymentAsync(string id, TableOpeningFamily tableOpeningFamilyDTO, string typeRequest)
        {
            return string.Empty;
        }

        private async Task<string> SaveRequestPayment(string id, TableOpeningFamily tableOpeningFamilyDTO, string typeRequest, Store store, PrintSettings sector)
        {
            return string.Empty;
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
