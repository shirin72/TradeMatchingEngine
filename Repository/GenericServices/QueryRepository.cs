using Infrastructure.Order.CommandRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.GenericRepositories;

namespace Infrastructure.GenericServices
{
    public class QueryRepository<T, TInterface> : IQueryRepository<T, TInterface> where T : class, TInterface
    {
        protected readonly DbContext _dbContext;
        protected readonly IQueryable<T> _querySet;
        public QueryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _querySet = dbContext.Set<T>().AsNoTracking();
        }
        public async Task<IEnumerable<TInterface>> GetAll(Expression<Func<T, bool>>? predicate = null)
        {
            return await (predicate != null ? _querySet.Where(predicate) : _querySet).ToListAsync();
        }

        public async Task<TInterface?> Get(Expression<Func<T, bool>> predicate)
        {
            return await _querySet.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<long> GetMax(Expression<Func<T, long?>> selector)
        {
            return await _querySet.MaxAsync(selector) ?? 0;
        }
    }
}
