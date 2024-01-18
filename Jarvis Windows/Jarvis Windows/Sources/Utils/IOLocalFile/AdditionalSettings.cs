using System;
using System.IO;
using Windows.ApplicationModel;

// JsonObject static class contains static JsonObject that will stores value from LocalSettings (../IOLocalFile.cs)
// JsonObject static class also contains several methods that assist for sendingGA4 events.
// Logging class is for logging

public static class JsonObject
{
    private const int SESSION_EXPIRATION_IN_MIN = 30;
    public static string ClientID { get; set; }
    public static string UserID { get; set; }
    public static string SessionID { get; set; }
    public static string SessionTimestamp { get; set; }
    public static string AppVersion { get; set; }

    public static void Assign(string _clientID, string _userID, string _sessionID, string _sessionTimestamp, string _appVersion)
    {
        ClientID = _clientID;
        UserID = _userID;
        SessionID = _sessionID;
        SessionTimestamp = _sessionTimestamp;
        AppVersion = _appVersion;
    }

    // Get or create session data
    public static void GetOrCreateSessionData()
    {
        // Logging.Log($"About to create or get session ID, IOLocalFile status");

        long currentTimeInMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        long timeStamp = (SessionID != "") ? long.Parse(SessionID) : 0;

        if (!string.IsNullOrEmpty(SessionID) && timeStamp != 0)
        {
            long durationInMin = (currentTimeInMs - timeStamp) / 60000;
            if (durationInMin > SESSION_EXPIRATION_IN_MIN)
            {
                SessionID = "";
                SessionTimestamp = "0";
            }
            else
            {
                // Update timestamp to the latest
                SessionTimestamp = currentTimeInMs.ToString();
            }
        }

        // Assign current value
        if (string.IsNullOrEmpty(SessionID))
        {
            SessionID = currentTimeInMs.ToString();
            SessionTimestamp = currentTimeInMs.ToString();
        }
    }

    // Get app version using Windows.ApplicationModel
    public static string GetAppVersion()
    {
        Package package = Package.Current;
        PackageVersion version = package.Id.Version;
        string _curAppVersion = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        return _curAppVersion;

    }

    // For debugging
    public static string GetSettings()
    {
        string msg = $"{ClientID} - {UserID} - {SessionID} - {SessionTimestamp} - {AppVersion}";
        return msg;
    }
}

public static class Logging
{
    //private static bool isLogging = false;
    //public static void Log(string message)
    //{
    //    if (!isLogging) return;

    //    string _logFilePath = "C:\\Users\\pham.tan.anh.vu_styl\\Desktop\\logJarvis.txt";
    //    try 
    //    { 
    //        using (StreamWriter _streamWriter = new StreamWriter(_logFilePath, true))
    //        {
    //            _streamWriter.WriteLine($"{DateTime.Now} - {message}");
    //        }
    //    }

    //    catch { return; }
    //}
}