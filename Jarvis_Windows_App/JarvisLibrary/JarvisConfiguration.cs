using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace JarvisLibrary
{
    public static class JarvisConfiguration
    {
        #region Private Fields to get Configuration
        private static IConfigurationRoot GetConfiguration()
        {
            //run command before build: 'set JARVIS_ENVIRONMENT=dev' or staging, product in cases
            var environment = Environment.GetEnvironmentVariable("JARVIS_ENVIRONMENT") ?? "dev";
         
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", true, true);
            return builder.Build();
        }
        #endregion
    }
}
