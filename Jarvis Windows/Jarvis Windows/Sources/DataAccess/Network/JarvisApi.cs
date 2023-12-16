using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.DataAccess.Network;

public sealed class JarvisApi
{
    private static JarvisApi? _instance;

    const string _translateEndPoint = "translate";
    const string _reviseEndPoint = "revise";
    const string _shortenEndPoint = "shorten";

    private static HttpClient? _client;
    private static string? _apiUrl;

    private JarvisApi()
    {
        _client = new HttpClient();
        _apiUrl = DataConfiguration.ApiUrl;
    }

    public static JarvisApi Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new JarvisApi();
            }
            return _instance;
        }
    }

    public async Task<String?> ApiHandler(String requestBody, String endPoint)
    {
        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        try
        {
            HttpResponseMessage response = await _client.PostAsync(_apiUrl + endPoint, contentData);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            return null;
        }
        catch (Exception ex)
        {
            throw ex.GetBaseException();
        }
    }

    public async Task<string?> TranslateHandler(String content, String lang)
    {
        var requestBody = $"{{\"to\":\"{lang}\",\"content\":\"{content}\"}}";
        return await ApiHandler(requestBody, _translateEndPoint);
    }

    public async Task<string?> ShortenHandler(String content)
    {
        var requestBody = $"{{\"content\":\"{content}\"}}";
        return await ApiHandler(requestBody, _shortenEndPoint);

    }

    public async Task<string?> ReviseHandler(String content)
    {
        var requestBody = $"{{\"content\":\"{content}\"}}";
        return await ApiHandler(requestBody, _reviseEndPoint);
    }
}
