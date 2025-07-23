using System.Linq.Expressions;

namespace Abstractions.Repositories
{
    public interface IWriteRepository<T>
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities);
        Task<bool> DeleteAsync(Guid id);
        Task<(bool Success, int Count)> DeleteAsync(Expression<Func<T, bool>> query);
    }
}
