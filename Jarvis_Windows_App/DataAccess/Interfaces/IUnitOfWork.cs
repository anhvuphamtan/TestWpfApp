using System;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        #region Repositories
        //IGenericRepository<Modle> Modles { get; }

        #endregion

        /// <summary>
        /// Save changes to database
        /// </summary>
        /// <returns></returns>
        Task<int> Save();
    }
}
