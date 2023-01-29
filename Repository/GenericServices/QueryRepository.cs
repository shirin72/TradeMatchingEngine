using Infrastructure.Order.CommandRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TradeMatchingEngine.GenericRepositories;

namespace Infrastructure.GenericServices
{
    public class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _querySet;
        public QueryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _querySet = dbContext.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _querySet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetById(Expression<Func<T, bool>> expression)
        {
            return await _querySet.AsNoTracking().Where(expression).FirstOrDefaultAsync();
        }

        public async Task<long> GetLastId()
        {
            if (_querySet.Any())
            {
                var obj = new { Id = 0 };

                Expression<Func<T, long>> expression = ((x) => obj.Id);

                return await _querySet.MaxAsync(expression);
            }

            return 0;
        }
    }
}
