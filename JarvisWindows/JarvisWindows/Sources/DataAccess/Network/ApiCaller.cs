using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWindows.Sources.DataAccess.Network
{
    public sealed class ApiCaller
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ApiCaller instance = new ApiCaller();
        private static string apiUrl = DataConfiguration.ApiUrl;

        private ApiCaller()
        {
            // Private constructor to prevent external instantiation
        }

        public static ApiCaller Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<string?> ConnectToDatabaseAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return null;
            }
        }
    }
}
