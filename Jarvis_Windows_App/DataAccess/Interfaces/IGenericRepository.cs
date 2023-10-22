using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Get Entity with id
        /// </summary>
        /// <param name="id">The id of entity</param>
        /// <returns>Entity or null</returns>
        Task<T> GetAsync(string id);

        /// <summary>
        /// Get Entity with id
        /// </summary>
        /// <param name="id">The id of entity</param>
        /// <param name="otherEntities">Other Foreign Keys to be included</param>
        /// <returns>Entity or null</returns>
        Task<T> GetAsync(string id, params string[] otherEntities);

        /// <summary>
        /// Get List of entities
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get List of entities
        /// </summary>
        /// <param name="otherEntities">Other Foreign Keys to be included</param>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(params string[] otherEntities);

        /// <summary>
        /// Add new entity to database
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        /// <returns>The added entity</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(T entity);

        /// <summary>
        /// Delete entity with only id specified
        /// </summary>
        /// <param name="id">Id of entity to be deleted</param>
        void Delete(string id);

        /// <summary>
        /// Delete multiple entities in the same table
        /// </summary>
        /// <param name="entities">Entities to be removed</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">New information of entity to be updated</param>
        void Update(T entity);

        /// <summary>
        /// Execute Query for Select Command
        /// </summary>
        /// <param name="sqlQuery">SELECT command to execute</param>
        /// <returns>List of entities that satisfies the command</returns>
        Task<IEnumerable<T>> ExecuteQueryAsync(string sqlQuery);
    }
}
