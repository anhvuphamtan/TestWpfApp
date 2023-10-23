using Jarvis_Windows.Sources.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.Services
{
    public class SimpleService
    {
        private readonly ApiCaller _apiCaller;

        public SimpleService(ApiCaller databaseConnector)
        {
            _apiCaller = databaseConnector;
        }

        /// <summary>
        /// Sample call API method
        /// </summary>
        /// <returns></returns>
        public async Task<string> PostDataToApi()
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer YOUR_TOKEN" },
                { "Content-Type", "application/json" }
            };
            string requestBody = "{ \"key\": \"value\" }";

            string apiResponse = await _apiCaller.CallApi(DataConfiguration.ConnectionString, headers, requestBody);

            // Process the API response
            return apiResponse;
        }
    }
}
