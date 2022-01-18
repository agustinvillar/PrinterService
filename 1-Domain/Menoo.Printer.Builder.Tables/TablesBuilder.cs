﻿using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Constants;
using Menoo.PrinterService.Infraestructure.Database.Firebase.Entities;
using Menoo.PrinterService.Infraestructure.Interfaces;
using Menoo.PrinterService.Infraestructure.Queues;
using Menoo.PrinterService.Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Menoo.Printer.Builder.Tables
{
    [Handler]
    public class TablesBuilder : ITicketBuilder
    {
        private const int PRINT_MINUTES = 10;

        private const int MIN_SHARED_TABLES_OPENING = 2;

        private readonly EventLog _generalWriter;

        private readonly TableOpeningFamilyRepository _tableOpeningFamilyRepository;

        private readonly StoreRepository _storeRepository;

        private readonly PaymentRepository _paymentRepository;

        public TablesBuilder(
            StoreRepository storeRepository,
            TableOpeningFamilyRepository tableOpeningFamilyRepository,
            PaymentRepository paymentRepository)
        {
            _storeRepository = storeRepository;
            _tableOpeningFamilyRepository = tableOpeningFamilyRepository;
            _paymentRepository = paymentRepository;
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
                dataToPrint.Content = await GetInfoCloseTableOpeningFamilyAsync(tableOpeningFamilyDTO);
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
        private async Task<Dictionary<string, object>> GetInfoCloseTableOpeningFamilyAsync(TableOpeningFamily tableOpeningFamilyDTO)
        {
            var data = new Dictionary<string, object>();
            var dateTime = Convert.ToDateTime(tableOpeningFamilyDTO.ClosedAt);
            bool isOnlyUser = tableOpeningFamilyDTO.TableOpenings.Count > 0 && tableOpeningFamilyDTO.TableOpenings.Count < MIN_SHARED_TABLES_OPENING;
            data.Add("title", SetTitleForCloseTable(tableOpeningFamilyDTO));
            data.Add("tableNumber", tableOpeningFamilyDTO.TableNumberToShow.ToString());
            data.Add("closedAt", dateTime.ToString("dd/MM/yyyy HH:mm:ss"));
            data.Add("isOnlyUser", isOnlyUser);
            if (isOnlyUser)
            {
                var tableOpeningInfo = tableOpeningFamilyDTO.TableOpenings.FirstOrDefault();
                var ordersActive = tableOpeningInfo.Orders.FindAll(f => !f.Status.ToLower().Contains("cancelado"));
                var paymentData = await _paymentRepository.GetPaymentByIdAsync(tableOpeningInfo.PaymentId.GetValueOrDefault());
                var orderUnified = GetOrderData(ordersActive);
                var subTotal = GetSubtotal(orderUnified);
                var propina = tableOpeningInfo.Propina != null && tableOpeningInfo.Propina.GetValueOrDefault() > 0 ? Convert.ToDecimal(tableOpeningInfo.Propina).ToString("N2", CultureInfo.CreateSpecificCulture("en-US")) : string.Empty;
                var paymentSurcharge = paymentData.Surcharge != null && paymentData.Surcharge.GetValueOrDefault() > 0 ? Convert.ToDecimal(paymentData.Surcharge.GetValueOrDefault()).ToString("N2", CultureInfo.CreateSpecificCulture("en-US")) : string.Empty;
                data.Add("userName", tableOpeningInfo.UserName);
                data.Add("orderData", orderUnified);
                data.Add("subTotal", subTotal);
                data.Add("paymentData", paymentData);
                data.Add("propina", propina);
                data.Add("paymentSurcharge", paymentSurcharge);

            }
            else
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
            var extras = new List<Extra>();
            var ordersByStore = orders.GroupBy(g => g.Store.Id).FirstOrDefault();
            var ordersByAddress = orders.GroupBy(g => g.Address).FirstOrDefault();
            var ordersByUsername = orders.GroupBy(g => g.UserName).FirstOrDefault();
            orderUnified.Address = ordersByAddress.Key;
            orderUnified.UserName = ordersByUsername.Key;
            orderUnified.Store = ordersByStore.ToList().FirstOrDefault().Store;
            orderUnified.OrderType = ordersByStore.ToList().FirstOrDefault().OrderType;
            orders.ForEach(item => {
                if (item.Items != null && item.Items.Count > 0) 
                {
                    items.AddRange(item.Items);
                }
                if (item.Extras != null && item.Extras.Count > 0) 
                {
                    extras.AddRange(item.Extras);
                }
            });
            orderUnified.Items = items;
            orderUnified.Extras = extras;
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

        private string GetSubtotal(Order orderData) 
        {
            double result = 0d;
            foreach (var item in orderData.Items)
            {
                result += item.Total.GetValueOrDefault();
            }
            var subtotal = Convert.ToDecimal(result).ToString("N2", CultureInfo.CreateSpecificCulture("en-US"));
            return subtotal;
        }
        #endregion
    }
}
