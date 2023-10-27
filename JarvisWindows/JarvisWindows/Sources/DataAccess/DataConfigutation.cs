using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace JarvisWindows.Sources.DataAccess
{
    public static class DataConfiguration
    {
        #region Private Fields to get Configuration
        private static IConfigurationRoot GetConfiguration()
        {
            // Determine the environment based on the conditional compilation symbols
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
                .AddJsonFile($"Envs/settings.{environment}.json", true, true);
            return builder.Build();
        }
        #endregion

        public static string ApiUrl
            => GetConfiguration()["ApiUrl"];
    }
}