using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine.GenericRepositories
{
    public interface IQueryRepository<T>
    {
        Task<T> GetById(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll();
        Task<long> GetLastId();
    }
}
