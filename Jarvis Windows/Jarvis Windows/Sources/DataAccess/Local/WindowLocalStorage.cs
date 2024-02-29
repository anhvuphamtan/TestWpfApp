using System.Collections.Generic;
using Windows.Storage;
using Hanssens.Net;
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
    private static bool _isPackaged = AppStatus.IsPackaged;
    private static ApplicationDataContainer LocalStoragePackage
    {
        get
        {
            if (_isPackaged) return ApplicationData.Current.LocalSettings;
            return null;
        }
    }

    private static LocalStorage? _localStorageUnpackage;
    private static LocalStorage LocalStorageUnpackage
    {
        get
        {
            if (_isPackaged) return null;

            if (_localStorageUnpackage == null)
            {
                var config = new LocalStorageConfiguration()
                {
                    Filename = $"{AppDomain.CurrentDomain.BaseDirectory}\\AppSettings\\LocalStorageUnpackage.localstorage",
                    AutoLoad = true,
                    AutoSave = true
                };

                _localStorageUnpackage = new LocalStorage(config);
            }

            return _localStorageUnpackage;
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

    public static void InitValue(string key)
    {
        if ( (_isPackaged && !LocalStoragePackage.Values.ContainsKey(key))
            || (!_isPackaged && !LocalStorageUnpackage.Exists(key)) ) 
            WriteLocalStorage(key, DefaultValue[key]);
    }

    public static string ReadLocalStorage(string key)
    {
        InitValue(key);
        if (_isPackaged) return LocalStoragePackage.Values[key].ToString();
        return LocalStorageUnpackage.Get<string>(key);
    }

    public static void WriteLocalStorage(string key, string value)
    {
        if (_isPackaged) LocalStoragePackage.Values[key] = value;
        else
        {
            LocalStorageUnpackage.Store(key, value);
            LocalStorageUnpackage.Persist();
        }
    }
}