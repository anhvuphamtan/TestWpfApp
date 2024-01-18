using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Jarvis_Windows.Sources.DataAccess
{
    public static class DataConfiguration
    {
        private static string _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
     
        #region Private Fields to get Configuration

        public static string GetEnvironment()
        {
            string environment;
            #if DEBUG
            environment = "dev";
            #elif RELEASE
            environment = "dev";
            #elif DEV
            environment = "dev";
            #elif PRODUCT
            environment = "product";
            #else
            #error "Invalid build configuration"
            #endif

            return environment;
        }
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_appDirectory)
                .AddJsonFile($"AppSettings/Envs/settings.{GetEnvironment()}.json", true, true);
            return builder.Build();
        }
        #endregion 

        // Debug mode only
        private static string GenUniqueID()
        {
            string newUID = Guid.NewGuid().ToString();            
            string jsonContent = File.ReadAllText(SettingFilePath);
            JObject jsonObj = JObject.Parse(jsonContent);
            jsonObj["ClientID"] = newUID;
            jsonObj["UserID"] = newUID;

            File.WriteAllText(SettingFilePath, jsonObj.ToString(Newtonsoft.Json.Formatting.Indented));

            return newUID;
        }

        // Root app directory
        public static string AppDirectory
            => _appDirectory;

        // settings.dev.json full path
        public static string SettingFilePath
            => Path.Combine(_appDirectory, $"AppSettings/Envs/settings.{GetEnvironment()}.json");
        public static string ApiUrl
            => GetConfiguration()["ApiUrl"];

        public static string MeasurementID
            => GetConfiguration()["MeasurementID"];

        public static string ApiSecret
            => GetConfiguration()["ApiSecret"];

        public static string ClientID
           => (GetConfiguration()["ClientID"] == "") ? GenUniqueID() : GetConfiguration()["ClientID"];
        public static string UserID
           => (GetConfiguration()["UserID"] == "") ? GenUniqueID() : GetConfiguration()["UserID"];
    }
}