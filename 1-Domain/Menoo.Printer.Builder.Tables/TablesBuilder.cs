using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Tables
{
    [Handler]
    public class TablesBuilder : ITicketBuilder
    {
        private readonly EventLog _generalWriter;

        private readonly TableOpeningFamilyRepository _tableOpeningFamilyRepository;

        private readonly StoreRepository _storeRepository;

        public TablesBuilder(
            StoreRepository storeRepository,
            TableOpeningFamilyRepository tableOpeningFamilyRepository)
        {
            _storeRepository = storeRepository;
            _tableOpeningFamilyRepository = tableOpeningFamilyRepository;
            _generalWriter = GlobalConfig.DependencyResolver.ResolveByName<EventLog>("builder");
        }

        public override string ToString()
        {
            return PrintBuilder.TABLE_BUILDER;
        }

        public async Task<List<PrintInfo>> BuildAsync(string id, PrintMessage data)
        {
            if (data.Builder != PrintBuilder.TABLE_BUILDER)
            {
                return null;
            }
            var tableOpeningFamilyDTO = await _tableOpeningFamilyRepository.GetById<TableOpeningFamily>(data.DocumentId, "tableOpeningFamily");
            var store = await _storeRepository.GetById<Store>(tableOpeningFamilyDTO.StoreId, "stores");
            var dataToPrint = new PrintInfo
            {
                Store = store
            };
            if (data.PrintEvent == PrintEvents.TABLE_OPENED)
            {
                dataToPrint.Content = GetInfoTableOpeningFamily(tableOpeningFamilyDTO);
                dataToPrint.BeforeAt = Utils.BeforeAt(tableOpeningFamilyDTO.OpenedAt, 10);
                dataToPrint.Template = PrintTemplates.TABLE_OPENED;
            }
            else if (data.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                dataToPrint.Content = GetInfoCloseTableOpeningFamily(tableOpeningFamilyDTO);
            }
            else if (data.PrintEvent == PrintEvents.REQUEST_PAYMENT)
            {
                dataToPrint.Content = GetInfoRequestPayment(tableOpeningFamilyDTO, data.SubTypeDocument);
            }
            return new List<PrintInfo>() { dataToPrint };
        }

        #region private methods
        private Dictionary<string, string> GetInfoCloseTableOpeningFamily(TableOpeningFamily tableOpeningFamilyDTO)
        {
            var data = new Dictionary<string, string>();
            return data;
        }

        private Dictionary<string, string> GetInfoTableOpeningFamily(TableOpeningFamily tableOpeningFamilyDTO)
        {
            var data = new Dictionary<string, string>();
            var dateTime = Convert.ToDateTime(tableOpeningFamilyDTO.OpenedAt);
            data.Add("title", "Apertura de mesa");
            data.Add("tableNumber", tableOpeningFamilyDTO.NumberToShow.ToString());
            data.Add("openedAt", dateTime.ToString("dd/MM/yyyy"));
            data.Add("timeAt", dateTime.ToString("HH:mm:ss"));
            return data;
        }

        private Dictionary<string, string> GetInfoRequestPayment(TableOpeningFamily tableOpeningFamilyDTO, string typeRequest)
        {
            var data = new Dictionary<string, string>();
            return data;
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
