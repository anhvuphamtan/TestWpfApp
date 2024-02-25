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
            environment = "product";
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
        public static void WriteValue(string key, object value)
        {
            string jsonContent = File.ReadAllText(SettingFilePath);
            JObject jsonObj = JObject.Parse(jsonContent);
            jsonObj[key] = JToken.FromObject(value);

            File.WriteAllText(SettingFilePath, jsonObj.ToString(Newtonsoft.Json.Formatting.Indented));
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
        {
            get
            {
                string clientID = GetConfiguration()["ClientID"];
                if (string.IsNullOrEmpty(clientID))
                    WriteValue("ClientID", Guid.NewGuid().ToString());
                
                return clientID;
            }
        }

        public static string UserID
        {
            get
            {
                string userID = GetConfiguration()["UserID"];
                if (string.IsNullOrEmpty(userID))
                    WriteValue("UserID", Guid.NewGuid().ToString());

                return userID;
            }
        }
        public static string SessionID
           => GetConfiguration()["SessionID"];
        public static string SessionTimestamp
           => GetConfiguration()["SessionTimestamp"];
        public static string ApiHeaderID
           => GetConfiguration()["ApiHeaderID"];
        public static int ApiUsageRemaining
           => int.Parse(GetConfiguration()["ApiUsageRemaining"]);
    }
}