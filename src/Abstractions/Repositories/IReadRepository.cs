using System.Linq.Expressions;

namespace Abstractions.Repositories
{
    public interface IReadRepository<T>
    {
        Task<T?> GetAsync(Guid id);
        Task<IEnumerable<T>> GetAsync();
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query);
    }
}
