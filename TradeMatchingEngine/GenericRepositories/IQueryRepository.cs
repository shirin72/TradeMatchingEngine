using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.GenericRepositories
{
    public interface IQueryRepository<T,TInterface>
    {
        Task<TInterface?> Get(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TInterface>> GetAll(Expression<Func<T, bool>>? predicate = null);
        Task<long> GetMax(Expression<Func<T, long?>> selector);
    }
}
