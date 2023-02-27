using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowTest
{
    public interface ICircuitBreaker
    {
        Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func);
    }
}
