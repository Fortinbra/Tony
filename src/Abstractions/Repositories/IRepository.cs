using System.Linq.Expressions;

namespace Abstractions.Repositories
{
    public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    {
        // Combined interface for full repository functionality
        // Inherits all methods from both read and write repositories
        IQueryable<T> AsQueryable();
    }
}
