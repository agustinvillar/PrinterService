using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
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
        private const int PRINT_MINUTES = 10;

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

        public async Task<PrintInfo> BuildAsync(string id, PrintMessage data)
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
                dataToPrint.BeforeAt = Utils.BeforeAt(tableOpeningFamilyDTO.OpenedAt, PRINT_MINUTES);
                dataToPrint.Template = PrintTemplates.TABLE_OPENED;
            }
            else if (data.PrintEvent == PrintEvents.TABLE_CLOSED)
            {
                dataToPrint.Content = GetInfoCloseTableOpeningFamily(tableOpeningFamilyDTO);
                dataToPrint.BeforeAt = Utils.BeforeAt(tableOpeningFamilyDTO.ClosedAt, PRINT_MINUTES);
                dataToPrint.Template = PrintTemplates.TABLE_CLOSED;
            }
            else if (data.PrintEvent == PrintEvents.REQUEST_PAYMENT)
            {
                dataToPrint.Content = GetInfoRequestPayment(tableOpeningFamilyDTO, data.SubTypeDocument);
            }
            return dataToPrint;
        }

        #region private methods
        private Dictionary<string, object> GetInfoCloseTableOpeningFamily(TableOpeningFamily tableOpeningFamilyDTO)
        {
            var data = new Dictionary<string, object>();
            var dateTime = Convert.ToDateTime(tableOpeningFamilyDTO.ClosedAt);
            data.Add("title", SetTitleForCloseTable(tableOpeningFamilyDTO));
            data.Add("tableNumber", tableOpeningFamilyDTO.TableNumberToShow.ToString());
            data.Add("closedAt", dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
            if (tableOpeningFamilyDTO.TableOpenings.Count == 1)
            {
                var tableOpeningInfo = tableOpeningFamilyDTO.TableOpenings.FirstOrDefault();
                data.Add("userName", tableOpeningInfo.UserName);
                var ordersActive = tableOpeningInfo.Orders.FindAll(f => f.Status.ToLower().Contains("cancelado"));
                data.Add("orderData", GetOrderData(ordersActive));
            }
            else if (tableOpeningFamilyDTO.TableOpenings.Count > 1)
            {
                data.Add("userName", string.Empty);
            }
            return data;
        }

        private Dictionary<string, object> GetInfoTableOpeningFamily(TableOpeningFamily tableOpeningFamilyDTO)
        {
            var data = new Dictionary<string, object>();
            var dateTime = Convert.ToDateTime(tableOpeningFamilyDTO.OpenedAt);
            data.Add("title", "Apertura de mesa");
            data.Add("tableNumber", tableOpeningFamilyDTO.TableNumberToShow.ToString());
            data.Add("openedAt", dateTime.ToString("dd/MM/yyyy"));
            data.Add("timeAt", dateTime.ToString("HH:mm:ss"));
            return data;
        }

        private Dictionary<string, object> GetInfoRequestPayment(TableOpeningFamily tableOpeningFamilyDTO, string typeRequest)
        {
            var data = new Dictionary<string, object>();
            return data;
        }

        private Order GetOrderData(List<Order> orders)
        {
            var orderUnified = new Order();
            var items = new List<ItemOrder>();
            var ordersByStore = orders.GroupBy(g => g.Store.Id).FirstOrDefault();
            var ordersByOrderNumber = orders.GroupBy(g => g.OrderNumber).FirstOrDefault();
            var ordersByAddress = orders.GroupBy(g => g.Address).FirstOrDefault();
            var ordersByUsername = orders.GroupBy(g => g.UserName).FirstOrDefault();
            var ordersStatus = orders.GroupBy(g => g.Status).FirstOrDefault();
            orderUnified.OrderNumber = ordersByOrderNumber.Key;
            orderUnified.Address = ordersByAddress.Key;
            orderUnified.UserName = ordersByUsername.Key;
            orderUnified.Store = ordersByStore.ToList().FirstOrDefault().Store;
            orderUnified.OrderType = ordersByStore.ToList().FirstOrDefault().OrderType;
            orderUnified.Status = ordersStatus.Key;
            orders.ForEach(item => {
                items.AddRange(item.Items);
            });
            orderUnified.Items = items;
            return orderUnified;
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
