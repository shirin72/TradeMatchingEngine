using Microsoft.EntityFrameworkCore;
using Domain.Orders.Repositories.Command;

namespace Infrastructure.Order.QueryRepositories
{
    public class CommandRepository<T, TInterface> : ICommandRepository<T, TInterface> where T : class, TInterface
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

        public async Task Add(TInterface order)
        {
            await Add((T)order);
        }

        public async Task Delete(long id)
        {
            var entiryToRemove = await _dbSet.FindAsync(id);
            _dbSet.Remove(entiryToRemove);
        }

        public async Task<TInterface> Find(long id)
        {
            var res = await _dbSet.FindAsync(id);

            return res;
        }
    }
}
