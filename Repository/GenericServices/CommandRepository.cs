using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class CommandRepository<T> : ICommandRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public CommandRepository(DbContext dbcontext)
        {
            _dbContext = dbcontext;
            _dbSet = dbcontext.Set<T>();
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public async Task Delete(long id)
        {
            var entiryToRemove = await _dbSet.FindAsync(id);
            _dbSet.Remove(entiryToRemove);
        }

        public async Task<T> Find(long id)
        {

            return await _dbSet.FindAsync(id);
        }
    }
}
