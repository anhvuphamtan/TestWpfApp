using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.DataAccess.Network;

public sealed class JarvisApi
{
    const string _translateEndPoint = "translate";
    const string _reviseEndPoint = "revise";
    const string _shortenEndPoint = "shorten";

    private static readonly HttpClient client = new();
    private static readonly JarvisApi instance = new();
    private static readonly string? apiUrl = DataConfiguration.ApiUrl;

    private JarvisApi()
    {
    }

    public static JarvisApi Instance
    {
        get
        {
            return instance;
        }
    }

    public static async Task<String?> ApiHandler(String requestBody, String endPoint)
    {
        var contentData = new StringContent(requestBody, Encoding.UTF8, "application/json");
        try
        {
            HttpResponseMessage response = await client.PostAsync(apiUrl + endPoint, contentData);
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

    public static async Task<string?> TranslateHandler(String content, String lang)
    {
        throw new NotImplementedException();
    }


    public static async Task<string?> ShortenHandler(String content)
    {
        throw new NotImplementedException();
    }


    public static async Task<string?> ReviseHandler(String content)
    {
        throw new NotImplementedException();
    }
}
