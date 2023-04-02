using Abstractions.Repositories;
using Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Mongo
{
    public class Repository<T> : IRepository<T>
        where T : Base
    {
        private readonly IMongoCollection<T> _collection;
        public Repository(IMongoDatabase mongoDb)
        {
            _collection = mongoDb.GetCollection<T>($"{typeof(T).Name}");
        }
        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public Task<T> CreateAsync(T entity)
        {
            try
            {
                _collection.InsertOne(entity);
                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Success, int Count)> DeleteAsync(Expression<T> query)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAsync(Expression<T> query, Expression<T> projection)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
