using DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepNhaVy
{
    public static class DependencyInjection
    {
        private static ServiceProvider? serviceProvider;

        public static ServiceProvider Services => serviceProvider;

        public static IUnitOfWork? Work => serviceProvider?.GetService<IUnitOfWork>();

        public static void Configureservices()
        {
            ServiceCollection services = new ServiceCollection();
            serviceProvider = services.BuildServiceProvider();
        }
    }
}
