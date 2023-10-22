using BusinessObject;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly JarvisContext context;
        protected readonly DbSet<T> dbSet;
      
        public GenericRepository(JarvisContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(string id)
        {
            T entity = GetAsync(id).Result;
            if (entity == null)
            {
                throw new Exception("Entity does not exist!!");
            }
            Delete(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync(string sqlQuery)
        {
            return null;
            //return await dbSet.FromSqlRaw(sqlQuery).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params string[] otherEntities)
        {
            IQueryable<T> entities = null;
            foreach (string other in otherEntities)
            {
                if (!string.IsNullOrEmpty(other))
                {
                    if (entities == null)
                    {
                        entities = dbSet.Include(other);
                    }
                    else
                    {
                        entities = entities.Include(other);
                    }
                }
            }
            return await entities.ToListAsync();
        }

        public async Task<T> GetAsync(string id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<T> GetAsync(string id, params string[] otherEntities)
        {
            IQueryable<T> entities = new List<T>()
            {
                await dbSet.FindAsync(id)
            }.AsQueryable();

            if (entities.Any())
            {
                foreach (string other in otherEntities)
                {
                    if (!string.IsNullOrEmpty(other))
                    {
                        entities = entities.Include(other);
                    }
                }
            }
            return entities.FirstOrDefault();
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
