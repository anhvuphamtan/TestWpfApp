using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Jarvis_Windows.Sources.DataAccess;
using System.Text.Json;
using System.Windows;

public class SendEventGA4
{
    private const int DEFAULT_ENGAGEMENT_TIME_IN_MSEC = 100;
    private IOLocalFile IOLocalFile { get; set; }
    private bool _isPackaged; // Check if running in package mode

    private readonly HttpClient _httpClient;
    private readonly string _ga4Endpoint;
    private readonly string _measurementID;
    private readonly string _apiSecret;
    private readonly string _clientID;
    private readonly string _userID;
    private string _sessionID;
    private string _timeStamp;
    private string _version; // App version, only available in package mode

    private FileSetting _fileSetting; // Debug mode only

    public SendEventGA4()
    {
        _httpClient = new HttpClient();
        _ga4Endpoint = "https://www.google-analytics.com/mp/collect";
        _measurementID = DataConfiguration.MeasurementID;
        _apiSecret = DataConfiguration.ApiSecret;

        try
        {
            IOLocalFile = new IOLocalFile();
            _isPackaged = true;


            IOLocalFile.SetupMutableSettings();
            _clientID = JsonObject.ClientID;
            _userID = JsonObject.UserID;
            _sessionID = JsonObject.SessionID;
            _timeStamp = JsonObject.SessionTimestamp;
            _version = JsonObject.AppVersion;

        }
        catch (Exception ex)
        {
            _clientID = DataConfiguration.ClientID;
            _userID = DataConfiguration.UserID;
            
            _fileSetting = new FileSetting();
            _sessionID = _fileSetting.FileSessionID;
            _timeStamp = _fileSetting.FileSessionTimestamp;

        }
    }

    public async Task SendEvent(string eventName, Tuple<string, string> eventParams = null)
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
        catch (Exception ex)
        {
            throw;
        }
    }

    // CheckVersion of App, package mode only. This will be called in ExecuteCheckUpdate() in MainViewModel.cs constructor
    public async Task CheckVersion()
    {

        string _recentVersion = "";
        try { _recentVersion = JsonObject.GetAppVersion(); }
        catch (Exception ex)
        {
            return;
        }
        if (_version == "")
        {
            _version = JsonObject.AppVersion = _recentVersion;
            await SendEvent("windows_app_installed");
        }

        else if (_version != _recentVersion)
        {
            _version = JsonObject.AppVersion = _recentVersion;
            await SendEvent("windows_app_updated");
        }
    }

    private JObject CreateEventPayload(string eventName, Tuple<string, string> eventParams = null)
    {

        // GetOrCreateSessionData is different for debug/package mode.
        if (_isPackaged)
        {
            JsonObject.GetOrCreateSessionData();
            _sessionID = JsonObject.SessionID;
            _timeStamp = JsonObject.SessionTimestamp;
            IOLocalFile.WriteMutableSettings();
        }

        else
        {
            _fileSetting.GetOrCreateSessionData();
            _sessionID = _fileSetting.FileSessionID;
            _timeStamp = _fileSetting.FileSessionTimestamp;
            _fileSetting.WriteMutableSettings();
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

        if (eventParams != null)
        {
            payload["events"][0]["params"]["ai_action"] = eventParams.Item1;
            if (!string.IsNullOrEmpty(eventParams.Item2))
            {
                payload["events"][0]["params"]["ai_action_translate_to"] = eventParams.Item2;
            }
        }

        return payload;
    }
}

public class FileSetting
{
    private SessionData _sessionData;
    private const int SESSION_EXPIRATION_IN_MIN = 30;

    private string _sessionID;
    private string _timeStamp;

    public string FileSessionID 
    { 
        get => _sessionID; 
        set 
        { 
            _sessionID = value; 
        } 
    }
    public string FileSessionTimestamp 
    { 
        get => _timeStamp; 
        set 
        {
            _timeStamp = value; 
        } 
    }

    public FileSetting()
    {
        _sessionData = new SessionData();
        ReadMutableSettings();
    }

    // Read from settings.dev.json
    public void ReadMutableSettings()
    {
        string jsonContent = File.ReadAllText(DataConfiguration.SettingFilePath);
        _sessionData = JsonSerializer.Deserialize<SessionData>(jsonContent);

        FileSessionID = _sessionData.SessionID;
        FileSessionTimestamp = _sessionData.SessionTimestamp;

    }

    public void GetOrCreateSessionData()
    {
        long currentTimeInMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        long timeStamp = (FileSessionID != "") ? long.Parse(FileSessionID) : 0;

        if (!string.IsNullOrEmpty(FileSessionID) && timeStamp != 0)
        {
            long durationInMin = (currentTimeInMs - timeStamp) / 60000;
            if (durationInMin > SESSION_EXPIRATION_IN_MIN)
            {
                FileSessionID = "";
                FileSessionTimestamp = "0";
            }
            else
            {
                // Update timestamp to the latest
                FileSessionTimestamp = currentTimeInMs.ToString();
            }
        }

        // Assign current value
        if (string.IsNullOrEmpty(FileSessionID))
        {
            FileSessionID = currentTimeInMs.ToString();
            FileSessionTimestamp = currentTimeInMs.ToString();
        }
    }


    // Write to settings.dev.json
    public void WriteMutableSettings()
    {
        _sessionData.SessionID = FileSessionID;
        _sessionData.SessionTimestamp = FileSessionTimestamp;

        string _jsonString = JsonSerializer.Serialize(_sessionData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataConfiguration.SettingFilePath, _jsonString);
    }
}

public class SessionData
{
    public string ApiUrl { get; set; }
    public string MeasurementID { get; set; }
    public string ApiSecret { get; set; }
    public string ClientID { get; set; }
    public string UserID { get; set; }
    public string SessionID { get; set; }
    public string SessionTimestamp { get; set; }
}