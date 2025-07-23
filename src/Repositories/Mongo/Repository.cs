using Abstractions.Repositories;
using Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Repositories.Mongo
{
    public class Repository<T> : ReadOnlyRepository<T>, IRepository<T>
        where T : Base
    {
        public Repository(IMongoDatabase mongoDb) : base(mongoDb)
        {
        }

        public IQueryable<T> AsQueryable()
        {
            return Collection.AsQueryable();
        }

        public async Task<T> CreateAsync(T entity)
        {
            try
            {
                await Collection.InsertOneAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities)
        {
            try
            {
                var entitiesList = entities.ToList();
                await Collection.InsertManyAsync(entitiesList);
                return entitiesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<T> UpdateAsync(T entity)
        {
            try
            {
                await Collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities)
        {
            try
            {
                var entitiesList = entities.ToList();
                var bulkOps = entitiesList.Select(entity => 
                    new ReplaceOneModel<T>(
                        Builders<T>.Filter.Eq(x => x.Id, entity.Id), 
                        entity));
                
                await Collection.BulkWriteAsync(bulkOps);
                return entitiesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var result = await Collection.DeleteOneAsync(x => x.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<(bool Success, int Count)> DeleteAsync(Expression<Func<T, bool>> query)
        {
            try
            {
                var result = await Collection.DeleteManyAsync(query);
                return (result.DeletedCount > 0, (int)result.DeletedCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
