﻿using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Exceptions;
using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Client
{
    public class ApiClient
    {
        private readonly RestClient _restClient;

        public ApiClient()
        {
            string baseUrl = ConfigurationManager.AppSettings["api"].ToString();
            _restClient = new RestClient(baseUrl);
        }

        public async Task<Guid> ConfigurePrinter(ConfigurePrinterRequest configuration)
        {
            var request = new RestRequest(ApiResources.CONFIGURE, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(configuration);
            var response = await _restClient.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            var entityId = response.Content.ToObject<PrinterConfiguredDTO>();
            return new Guid(entityId.RegistrationId);
        }

        public async Task<bool> IsExistsConfiguration(string storeId)
        {
            var request = new RestRequest(string.Format(ApiResources.EXISTS_CONFIGURATION, storeId), Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var response = await _restClient.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            var isExists = response.Content.ToObject<ExistsConfigurationDTO>();
            return isExists.Exists;
        }

        public async Task<List<PrintEventsDTO>> GetAllPrintEventsAsync()
        {
            List<PrintEventsDTO> printEvents = new List<PrintEventsDTO>();
            var request = new RestRequest(ApiResources.PRINTER_EVENTS, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var response = await _restClient.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            printEvents.AddRange(response.Content.ToObject<List<PrintEventsDTO>>());
            return printEvents;
        }

        public async Task<List<StoreInfoDTO>> GetAllStoresAsync()
        {
            List<StoreInfoDTO> stores = new List<StoreInfoDTO>()
            {
                new StoreInfoDTO
                {
                    BusinessName = string.Empty,
                    StoreAuxId = string.Empty,
                    StoreName = AppMessages.Empty
                }
            };
            var request = new RestRequest(ApiResources.STORES, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var response = await _restClient.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            stores.AddRange(response.Content.ToObject<List<StoreInfoDTO>>());
            return stores;
        }
    }
}
