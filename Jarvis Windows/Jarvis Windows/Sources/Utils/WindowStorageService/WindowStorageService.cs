using System;
using Windows.ApplicationModel;

public static class AppStatus
{
    public static bool IsPackaged => CheckIsPackaged();

    private static bool CheckIsPackaged()
    {
        try { return Windows.Storage.ApplicationData.Current.LocalSettings != null; }
        catch { return false; }
    }
}

public class WindowStorageService
{
    public Windows.Storage.ApplicationDataContainer LocalSettings { get; set; }
    public WindowStorageService()
    {
        LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
    }

    public string ReadLocalStorage(string key)
    {
        return LocalSettings.Values.ContainsKey(key) ? LocalSettings.Values[key].ToString() : string.Empty;
    }

    public void WriteLocalStorage(string key, string value)
    {
        LocalSettings.Values[key] = value;    
    }

    public string InitializeAndWrite(string key, string defaultValue)
    {
        string value = ReadLocalStorage(key);
        if (string.IsNullOrEmpty(value)) value = defaultValue;

        WriteLocalStorage(key, value);
        return value;
    }
}

public class GA4LocalStorage : WindowStorageService
{
    private const int SESSION_EXPIRATION_IN_MIN = 30;
    public string ClientID { get; set; }
    public string UserID { get; set; }
    public string SessionID { get; set; }
    public long SessionTimestamp { get; set; }
    public string AppVersion { get; set; }

    public GA4LocalStorage()
    {
        ClientID = InitializeAndWrite("ClientID", Guid.NewGuid().ToString());
        UserID = InitializeAndWrite("UserID", Guid.NewGuid().ToString());
        SessionID = InitializeAndWrite("SessionID", "");
        SessionTimestamp = long.Parse(InitializeAndWrite("SessionTimestamp", "0"));
        AppVersion = InitializeAndWrite("AppVersion", "");
    }

    public void GetOrCreateSessionData()
    {
        long _currentTimeInMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        if (string.IsNullOrEmpty(SessionID))
        {
            SessionID = _currentTimeInMs.ToString();
            SessionTimestamp = _currentTimeInMs;
        }

        else if (!string.IsNullOrEmpty(SessionID) && SessionTimestamp != 0)
        {
            long _durationInMin = (_currentTimeInMs - SessionTimestamp) / 60000;
            if (_durationInMin > SESSION_EXPIRATION_IN_MIN)
            {
                SessionID = "";
                SessionTimestamp = 0;
            }
            else SessionTimestamp = _currentTimeInMs;
        }
    }
    public string GetAppVersion()
    {
        Package package = Package.Current;
        PackageVersion version = package.Id.Version;
        string _curAppVersion = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        return _curAppVersion;
    }
}

public class APILocalStorage : WindowStorageService
{
    public string ApiHeaderID { get; set; }
    public int ApiUsageRemaining { get; set; }

    public APILocalStorage()
    {
        ApiHeaderID = InitializeAndWrite("ApiHeaderID", "");
        ApiUsageRemaining = int.Parse(InitializeAndWrite("ApiUsageRemaining", "0"));
    }
}
