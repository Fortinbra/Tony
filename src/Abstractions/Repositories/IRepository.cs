﻿using System.Linq.Expressions;

namespace Abstractions.Repositories
{
    public interface IRepository<T>
    {
        public Task<T> CreateAsync(T entity);
        public Task<IEnumerable<T>> CreateAsync(IEnumerable<T> entities);
        public Task<T> GetAsync(Guid id);
        public Task<IEnumerable<T>> GetAsync();
        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> query);
        public Task<T> UpdateAsync(T entity);
        public Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities);
        public Task<bool> DeleteAsync(Guid id);
        public Task<(bool Success, int Count)> DeleteAsync(Expression<Func<T, bool>> query);

        public IQueryable<T> AsQueryable();

    }
}
