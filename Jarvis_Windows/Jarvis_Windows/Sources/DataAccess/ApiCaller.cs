using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.DataAccess
{
    public sealed class ApiCaller
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ApiCaller instance = new ApiCaller();

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

        public async Task<string?> ConnectToDatabaseAsync(string apiUrl)
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

        public async Task<string?> CallApi(string apiUrl, Dictionary<string, string> headers, string requestBody)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

                // Add headers to the request
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // Add request body
                if (!string.IsNullOrEmpty(requestBody))
                {
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                // Handle exception
                Console.WriteLine($"Error calling API: {ex.Message}");
                return null;
            }
        }
    }
}
