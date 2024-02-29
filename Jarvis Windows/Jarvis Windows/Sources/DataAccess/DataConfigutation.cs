using Microsoft.Extensions.Configuration;
using System;

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

        // Root app directory
        public static string AppDirectory
            => _appDirectory;
        public static string ApiUrl
            => GetConfiguration()["ApiUrl"];

        public static string MeasurementID
            => GetConfiguration()["MeasurementID"];

        public static string ApiSecret
            => GetConfiguration()["ApiSecret"];
    }
}