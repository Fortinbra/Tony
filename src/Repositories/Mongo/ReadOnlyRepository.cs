using Abstractions.Repositories;
using Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Repositories.Mongo
{
    public class ReadOnlyRepository<T> : IReadRepository<T>
        where T : Base
    {
        protected readonly IMongoCollection<T> Collection;

        public ReadOnlyRepository(IMongoDatabase mongoDb)
        {
            Collection = mongoDb.GetCollection<T>($"{typeof(T).Name}");
        }

        public virtual async Task<T?> GetAsync(Guid id)
        {
            return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query)
        {
            return await Collection.Find(query).ToListAsync();
        }
    }
}
