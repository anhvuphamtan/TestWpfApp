using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Jarvis_Windows.Sources.DataAccess
{
    public static class DataConfiguration
    {
        #region Private Fields to get Configuration
        private static IConfigurationRoot GetConfiguration()
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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"AppSettings/Envs/settings.{environment}.json", true, true);
            return builder.Build();
        }
        #endregion

        public static string ApiUrl
            => GetConfiguration()["ApiUrl"];
    }
}