using FlixTv.Api.Application.Interfaces.Repositories;
using FlixTv.Api.Domain.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : class, IEntityBase, new()
    {
        private readonly DbContext dbContext;

        private DbSet<T> Table { get => dbContext.Set<T>(); }

        public ReadRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false)
        {
            IQueryable<T> queryable = Table;

            if (!enableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);

            if (predicate is not null) queryable = queryable.Where(predicate);

            if (orderBy is not null) return await orderBy(queryable).ToListAsync();

            return await queryable.ToListAsync();
        }

        public async Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false,
            int currentPage = 1,
            int pageSize = 10)
        {
            IQueryable<T> queryable = Table;

            if (!enableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);

            if (predicate is not null) queryable = queryable.Where(predicate);

            if (orderBy is not null) return await orderBy(queryable).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();

            return await queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool enableTracking = false)
        {
            IQueryable<T> queryable = Table;

            if (!enableTracking) queryable = queryable.AsNoTracking();

            if (include is not null) queryable = include(queryable);

            return await queryable.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> queryable = Table;

            queryable = queryable.AsNoTracking();

            if (predicate is not null) queryable = queryable.Where(predicate);

            return await queryable.CountAsync();
        }
    }
}