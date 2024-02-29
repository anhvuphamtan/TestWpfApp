using System.Collections.Generic;
using Windows.Storage;
using Hanssens.Net;
using System.IO;
using System;

public static class AppStatus
{
    public static bool IsPackaged => CheckIsPackaged();
    private static bool CheckIsPackaged()
    {
        try { return ApplicationData.Current.LocalSettings != null; }
        catch { return false; }
    }
}

public static class WindowLocalStorage
{
    private static LocalStorage? _localStorage;
    private static LocalStorage LocalStorage
    {
        get
        {
            string _fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\AppSettings\\LocalStorageFile.localstorage";
            if (File.Exists(_fileName))
                File.SetAttributes(_fileName, System.IO.FileAttributes.Normal);
            
            if (_localStorage == null)
            {
                var config = new LocalStorageConfiguration()
                {
                    Filename = _fileName,
                    AutoLoad = true,
                    AutoSave = true
                };

                _localStorage = new LocalStorage(config);
            }

            return _localStorage;
        }
    }

    private static Dictionary<string, string> DefaultValue = new Dictionary<string, string>
    {
        { "ClientID", Guid.NewGuid().ToString() },
        { "UserID", Guid.NewGuid().ToString() },
        { "SessionID", "" },
        { "SessionTimestamp", "0" },
        { "AppVersion", "" },
        { "ApiHeaderID", Guid.NewGuid().ToString() },
        { "ApiUsageRemaining", "10" }
    };

    public static void InitValue(string key)
    {
        if (!LocalStorage.Exists(key)) 
            WriteLocalStorage(key, DefaultValue[key]);
    }

    public static string ReadLocalStorage(string key)
    {
        InitValue(key);
        return LocalStorage.Get<string>(key);
    }

    public static void WriteLocalStorage(string key, string value)
    {
        LocalStorage.Store(key, value);
        LocalStorage.Persist();
        
    }
}
