using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Jarvis_Windows.Sources.DataAccess;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows;

public class SendEventGA4
{
    private const int DEFAULT_ENGAGEMENT_TIME_IN_MSEC = 100;
    private const int SESSION_EXPIRATION_IN_MIN = 30;
    // private GA4LocalStorage2 GA4LocalStorage { get; set; } // Package mode only
    
    private readonly HttpClient _httpClient;
    private readonly string _ga4Endpoint;
    private readonly string _measurementID;
    private readonly string _apiSecret;
    private readonly string _clientID;
    private readonly string _userID;
    private string _sessionID;
    private long _sessionTimestamp;
    private string _version; // App version, only available in package mode

    
    public SendEventGA4()
    {
        _httpClient = new HttpClient();
        _ga4Endpoint = "https://www.google-analytics.com/mp/collect";
        _measurementID = DataConfiguration.MeasurementID;
        _apiSecret = DataConfiguration.ApiSecret;

        try
        {
            // GA4LocalStorage = new GA4LocalStorage2();
            _clientID = WindowStorageService2.ReadLocalStorage("ClientID");
            _userID = WindowStorageService2.ReadLocalStorage("UserID");
            _sessionID = WindowStorageService2.ReadLocalStorage("SessionID");
            _sessionTimestamp = long.Parse(WindowStorageService2.ReadLocalStorage("SessionTimestamp"));
            _version = WindowStorageService2.ReadLocalStorage("Version");

        }
        catch
        {
            _clientID = DataConfiguration.ClientID;
            _userID = DataConfiguration.UserID;
            _sessionID = DataConfiguration.SessionID;
            _sessionTimestamp = long.Parse(DataConfiguration.SessionTimestamp);
        }
    }

    public async Task SendEvent(string eventName, Dictionary<string, object> eventParams = null)
    {
        try
        {
            var body = CreateEventPayload(eventName, eventParams);
            var jsonBody = body.ToString();
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{_ga4Endpoint}?measurement_id={_measurementID}&api_secret={_apiSecret}",
                content
            );

            response.EnsureSuccessStatusCode();
        }
        catch { throw; }
    }

    // CheckVersion of App, package mode only. This will be called in ExecuteCheckUpdate() in MainViewModel.cs constructor
    public async Task CheckVersion()
    {
        if (!AppStatus.IsPackaged) return;

        string _recentVersion = "";
        try { _recentVersion = WindowStorageService2.GetAppVersion(); }
        catch { return; }
        
        if (_version == "")     
            await SendEvent("windows_app_installed");
        else if (_version != _recentVersion)
            await SendEvent("windows_app_updated");

        WindowStorageService2.WriteLocalStorage("AppVersion", _recentVersion);
        _version = _recentVersion;
    }

    private JObject CreateEventPayload(string eventName, Dictionary<string, object> eventParams)
    {
        if (AppStatus.IsPackaged)  // Package mode
        {
            WindowStorageService2.GetOrCreateSessionData();
            _sessionID = WindowStorageService2.ReadLocalStorage("SessionID");
            _sessionTimestamp = long.Parse(WindowStorageService2.ReadLocalStorage("SessionTimestamp"));
        }

        else // Debug mode
        {
            long _currentTimeInMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (string.IsNullOrEmpty(_sessionID))
            {
                _sessionID = _currentTimeInMs.ToString();
                _sessionTimestamp = _currentTimeInMs;
            }

            else if (!string.IsNullOrEmpty(_sessionID) && _sessionTimestamp != 0)
            {
                long _durationInMin = (_currentTimeInMs - _sessionTimestamp) / 60000;
                if (_durationInMin > SESSION_EXPIRATION_IN_MIN)
                {
                    _sessionID = "";
                    _sessionTimestamp = 0;
                }
                else _sessionTimestamp = _currentTimeInMs;
            }
            
            DataConfiguration.WriteValue("SessionID", _sessionID);
            DataConfiguration.WriteValue("SessionTimestamp", _sessionTimestamp);
        }

        var payload = JObject.FromObject(new
        {
            client_id = _clientID,
            user_id = _userID,
            events = new[]
            {
                new
                {
                    name = eventName,
                    @params = new
                    {
                        session_id = _sessionID,
                        debug_mode = 1,
                        engagement_time_msec = DEFAULT_ENGAGEMENT_TIME_IN_MSEC
                    }
                }
            }
        });

        if (eventParams == null) return payload;

        foreach (var kvp in eventParams)
        {
            string key = kvp.Key;
            object value = kvp.Value;

            payload["events"][0]["params"][key] = (string) value;
        }

        return payload;
    }
}