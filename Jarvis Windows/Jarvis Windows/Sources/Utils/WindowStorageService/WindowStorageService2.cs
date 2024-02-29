using Hanssens.Net;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;

public static class WindowStorageService2
{
    private const int SESSION_EXPIRATION_IN_MIN = 30;
    
    private static LocalStorage _localStorageInstance;
    public static LocalStorage LocalStorage
    {
        get
        {
            if (_localStorageInstance != null) return _localStorageInstance;
            
            var config = new LocalStorageConfiguration()
            {
                Filename = $"{AppDomain.CurrentDomain.BaseDirectory}\\AppSettings\\LocalStorage.localstorage",
                AutoLoad = true,
                AutoSave = true
            };

            _localStorageInstance = new LocalStorage(config);
            return _localStorageInstance;
        }
    }

    private static Dictionary<string, string> DefaultValue = new Dictionary<string, string>
    {
        { "ClientID", Guid.NewGuid().ToString() },
        { "UserID", Guid.NewGuid().ToString() },
        { "SessionID", "" },
        { "SessionTimestamp", "0" },
        { "AppVersion", "" },
        { "ApiHeaderID", "" },
        { "ApiUsageRemaining", "0" }
    };


    public static string ReadLocalStorage(string key)
    {
        InitValue(key, DefaultValue[key]);
        return LocalStorage.Get<string>(key);
        // return (LocalStorage.Exists(key)) ? LocalStorage.Get<string>(key) : string.Empty;
    }

    public static void WriteLocalStorage(string key, string value)
    {
        LocalStorage.Store(key, value);
        LocalStorage.Persist();
    }

    public static void InitValue(string key, string defaultValue)
    {
        if (!LocalStorage.Exists(key))
            WriteLocalStorage(key, defaultValue);
    }

    public static void GetOrCreateSessionData()
    {
        long _currentTimeInMs = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        string SessionID = ReadLocalStorage("SessionID");
        long SessionTimestamp = long.Parse(ReadLocalStorage("SessionTimestamp"));
        
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

        WriteLocalStorage("SessionID", SessionID);
        WriteLocalStorage("SessionTimeStamp", SessionTimestamp.ToString());
    }
    public static string GetAppVersion()
    {
        Package package = Package.Current;
        PackageVersion version = package.Id.Version;
        string _curAppVersion = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

        return _curAppVersion;
    }
}
