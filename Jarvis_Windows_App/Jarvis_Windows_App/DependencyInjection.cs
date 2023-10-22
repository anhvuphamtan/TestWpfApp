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
        private static ServiceProvider serviceProvider;

        public static ServiceProvider Services
        {
            get { return serviceProvider; }
        }

        public static IUnitOfWork Work
        {
            get { return serviceProvider.GetService<IUnitOfWork>(); }
        }

        public static void Configureservices()
        {
            ServiceCollection services = new ServiceCollection();

            #region DbContext
            services.AddDbContext<BnvContext>(options =>
            {
                options.UseSqlServer(BnvConfiguration.ConnectionString);
            });
            #endregion

            #region Repository
            services.AddScoped<IGenericRepository<Account>, GenericRepository<Account>>();
            #endregion

            #region UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
