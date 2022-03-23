using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Exceptions;
using Menoo.PrinterService.Client.Resources;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Client
{
    public class ApiClient
    {
        private readonly string _baseUrl;

        public ApiClient()
        {
            _baseUrl = ConfigurationManager.AppSettings["api"].ToString();
        }

        public async Task<List<StoreInfoDTO>> GetAllStoresAsync()
        {
            List<StoreInfoDTO> stores = new List<StoreInfoDTO>()
            {
                new StoreInfoDTO
                {
                    BusinessName = string.Empty,
                    StoreId = string.Empty,
                    StoreName = string.Empty
                }
            };
            string baseUrl = _baseUrl + ApiResources.STORES;
            var client = new RestClient(baseUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            var response = await client.ExecuteAsync(request);
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                throw new ApiException(response.ErrorMessage, response.ErrorException, response.StatusCode);
            }
            stores.AddRange(JsonConvert.DeserializeObject<List<StoreInfoDTO>>(response.Content));
            return stores;
        }
    }
}
