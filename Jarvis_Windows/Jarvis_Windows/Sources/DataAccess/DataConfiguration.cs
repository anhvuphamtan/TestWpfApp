using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.DataAccess
{
    public static class DataConfiguration
    {
        #region Private Fields to get Configuration
        private static IConfigurationRoot GetConfiguration()
        {
            //run command before build: 'set JARVIS_ENVIRONMENT=dev' or 'product' in each build case
            var environment = Environment.GetEnvironmentVariable("JARVIS_ENVIRONMENT") ?? "dev";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", true, true);
            return builder.Build();
        }
        #endregion

        public static string ConnectionString
            => GetConfiguration().GetConnectionString("ApiUrl");

        public static string AppName
            => GetConfiguration()["Common:AppName"];
    }
}
