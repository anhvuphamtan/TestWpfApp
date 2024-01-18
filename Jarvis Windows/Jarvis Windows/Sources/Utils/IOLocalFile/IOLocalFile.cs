using System;
using System.IO;
using System.Reflection;

// This class is for interacting with Local Setting using ApplicationDataContainer.
// Simply store LocalSettings[key] = value and data will persistent until app is removed.

public class IOLocalFile
{
    private Windows.Storage.ApplicationDataContainer _localSettings;

    public Windows.Storage.ApplicationDataContainer LocalSettings
    {
        get { return _localSettings; }
        set { _localSettings = value; }
    }
    public IOLocalFile()
    {
        LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        CheckKeyExist("ClientID", Guid.NewGuid().ToString());
        CheckKeyExist("UserID", Guid.NewGuid().ToString());
        CheckKeyExist("SessionID", "");
        CheckKeyExist("SessionTimestamp", "0");
        CheckKeyExist("AppVersion", "");
    }

    // SetupMutableSettings(), Init values from LocalSettings to static JsonObject class for easier
    public void SetupMutableSettings()
    {
        try
        {
            JsonObject.Assign(
                LocalSettings.Values["ClientID"].ToString(),
                LocalSettings.Values["UserID"].ToString(),
                LocalSettings.Values["SessionID"].ToString(),
                LocalSettings.Values["SessionTimestamp"].ToString(),
                LocalSettings.Values["AppVersion"].ToString()
            );

        }

        catch { }
    }

    // CheckKeyExist(keyName, value): Check if key is exist in LocalSettings, if not then register it with 'value'.
    public void CheckKeyExist(string keyName, string value)
    {
        if (!LocalSettings.Values.ContainsKey(keyName))
        {
            LocalSettings.Values[keyName] = value;
        }
    }


    // Update value in LocalSettings
    public void WriteMutableSettings()
    {
        LocalSettings.Values["ClientID"] = JsonObject.ClientID;
        LocalSettings.Values["UserID"] = JsonObject.UserID;
        LocalSettings.Values["SessionID"] = JsonObject.SessionID;
        LocalSettings.Values["SessionTimestamp"] = JsonObject.SessionTimestamp;
        LocalSettings.Values["AppVersion"] = JsonObject.AppVersion;

    }
}