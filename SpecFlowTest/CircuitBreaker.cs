using Newtonsoft.Json;
using System.Net;

namespace SpecFlowTest
{
    public class CircuitBreaker : ICircuitBreaker
    {
        private CircuitBreakerState _state;
        public CircuitBreaker()
        {
            _state = new CircuitBreakerClosed(this);
        }
        public Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func)
        {
            return _state.ExecuteService(input, func); ;
        }
        private abstract class CircuitBreakerState : ICircuitBreaker
        {
            protected CircuitBreaker _owner;
            public CircuitBreakerState(CircuitBreaker owner)
            {
                _owner = owner;
            }

            public abstract Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func);

        }
        private class CircuitBreakerClosed : CircuitBreakerState
        {

            private int _errorCount = 0;
            public CircuitBreakerClosed(CircuitBreaker owner)
                : base(owner) { }

            public override async Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func)
            {
                try
                {
                    return await func(input);
                }
                catch (Exception e)
                {
                    _trackErrors(e);
                    throw;
                }
            }


            private void _trackErrors(Exception e)
            {
                _errorCount += 1;
                if (_errorCount > Config.CircuitClosedErrorLimit)
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                }
            }
        }

        private class CircuitBreakerOpen : CircuitBreakerState
        {

            public CircuitBreakerOpen(CircuitBreaker owner)
                : base(owner)
            {
                Task.Run(async () => { 
                    await Task.Delay(Config.CircuitOpenTimeout);
                    owner._state = new CircuitBreakerHalfOpen(owner);
                });
            }

            public override async Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func)
            {
                throw new Exception("Service is not available");
            }

        }

        private class CircuitBreakerHalfOpen : CircuitBreakerState
        {
            private int _successCount = 0;
            private static readonly string Message = "Call failed when circuit half open";
            public CircuitBreakerHalfOpen(CircuitBreaker owner)
                : base(owner) { }


            public override async Task<TOutput> ExecuteService<TInput, TOutput>(TInput input, Func<TInput, Task<TOutput>> func)
            {
                try
                {
                    var result = await func(input);
                    _successCount += 1;

                    if (_successCount > Config.CircuitHalfOpenSuccessLimit)
                    {
                        _owner._state = new CircuitBreakerClosed(_owner);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                    throw new Exception(Message, e);
                }
            }
        }

    }
}

