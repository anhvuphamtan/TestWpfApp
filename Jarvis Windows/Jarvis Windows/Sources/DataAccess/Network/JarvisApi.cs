using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Jarvis_Windows.Sources.DataAccess.Network;

public sealed class JarvisApi
{
    private static JarvisApi? _instance;
    const string _endpoint = "/api/v1/ai-action/";

    private static HttpClient? _client;
    private static string? _apiUrl;
    // private APILocalStorage2 APILocalStorage { get; set; }

    private JarvisApi()
    {
        _client = new HttpClient();
        _apiUrl = DataConfiguration.ApiUrl;

        //try { APILocalStorage = new APILocalStorage2(); }
        //catch { }    
    }

    public static JarvisApi Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JarvisApi();
            }
            return _instance;
        }
    }

    public void StoreUsageRemaining(int value)
    {
        if (AppStatus.IsPackaged)
        {
            WindowStorageService2.WriteLocalStorage("ApiUsageRemaining", value.ToString());
            //APILocalStorage.ApiUsageRemaining = value;
            //APILocalStorage.WriteLocalStorage("ApiUsageRemaining", value.ToString());
        }
        else
            DataConfiguration.WriteValue("ApiUsageRemaining", value);
    }

    public async Task<string?> ApiHandler(string requestBody, string endPoint)
    {
        string _apiHeaderID = "";
        if (AppStatus.IsPackaged)
        {
            if (WindowStorageService2.ReadLocalStorage("ApiUsageRemaining") == "0")
            {
                //APILocalStorage.ApiHeaderID = ;
                //APILocalStorage.ApiUsageRemaining = 10;
                WindowStorageService2.WriteLocalStorage("ApiHeaderID", Guid.NewGuid().ToString());
                WindowStorageService2.WriteLocalStorage("ApiUsageRemaining", "10");
            }

            _apiHeaderID = WindowStorageService2.ReadLocalStorage("ApiHeaderID");

        }
        else
        {
            _apiHeaderID = (DataConfiguration.ApiHeaderID == "" || DataConfiguration.ApiUsageRemaining == 0) 
                            ? Guid.NewGuid().ToString() : DataConfiguration.ApiHeaderID;

            DataConfiguration.WriteValue("ApiHeaderID", _apiHeaderID);
            DataConfiguration.WriteValue("ApiUsageRemaining", 10);
        }
        
        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + endPoint);
            request.Content = contentData;
            request.Headers.Add("x-jarvis-guid", _apiHeaderID);
            HttpResponseMessage response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = JsonConvert.DeserializeObject(responseContent);

                int remainingUsage = responseObject.remainingUsage;
                StoreUsageRemaining(remainingUsage);

                string finalMessage = responseObject.message;
                return finalMessage;
            }

            return null;
        }
        catch (Exception ex)
        {
            throw ex.GetBaseException();
        }
    }


    // ---------------------------------- Non custom AI Actions ---------------------------------- //

    public async Task<string?> TranslateHandler(String content, String lang)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "translate",
            content = content,
            metadata = new
            {
                translateTo = lang
            }
        });

        return await ApiHandler(requestBody, _endpoint);
    }

    public async Task<string?> ReviseHandler(String content)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "revise",
            content = content
        });

        return await ApiHandler(requestBody, _endpoint);
    }

    public async Task<string?> AskHandler(String content, string action)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "ask",
            content = content,
            action = action
        });

        return await ApiHandler(requestBody, _endpoint);
    }

    public async Task<string?> AIHandler(string content, string action)
    {
        return await CustomAiActionHandler(content, action);
    }

    private async Task<string?> CustomAiActionHandler(string content, string action)
    {
        var requestBody = JsonConvert.SerializeObject(new
        {
            type = "custom",
            content = content,
            action = action
        });

        if (content == "")
        {
            requestBody = JsonConvert.SerializeObject(new
            {
                type = "custom",
                action = action
            });
        }


        return await ApiHandler(requestBody, _endpoint);
    }

    // ---------------------------------- Custom AI Actions ---------------------------------- //
}
