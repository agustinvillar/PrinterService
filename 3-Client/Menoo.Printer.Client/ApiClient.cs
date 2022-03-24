using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Exceptions;
using Menoo.PrinterService.Client.Extensions;
using Menoo.PrinterService.Client.Properties;
using Menoo.PrinterService.Client.Resources;
using RestSharp;
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

        public async Task<List<PrintEvents>> GetAllPrintEventsAsync()
        {
            List<PrintEvents> printEvents = new List<PrintEvents>();
            var request = new RestRequest(ApiResources.PRINTER_EVENTS, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var response = await _restClient.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            printEvents.AddRange(response.Content.ToObject<List<PrintEvents>>());
            return printEvents;
        }

        public async Task<List<StoreInfoDTO>> GetAllStoresAsync()
        {
            List<StoreInfoDTO> stores = new List<StoreInfoDTO>()
            {
                new StoreInfoDTO
                {
                    BusinessName = string.Empty,
                    StoreId = 0,
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
