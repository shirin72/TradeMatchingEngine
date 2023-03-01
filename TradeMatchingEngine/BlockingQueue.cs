using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public partial class BlockingQueue : IAsyncDisposable
    {
        private readonly BlockingCollection<ICommand> blockingCollection;
        private readonly Task mainTask;
        public BlockingQueue()
        {
            blockingCollection = new BlockingCollection<ICommand>();
            mainTask = Task.Run(async () =>
            {
                while (!blockingCollection.IsCompleted)
                {
                    try
                    {
                        var i = blockingCollection.Take();
                        await i.Execute();
                    }
                    catch (Exception ex)
                    {
                        if (ex is ObjectDisposedException || ex is InvalidOperationException)
                            continue;
                    }

                }
            });
        }

        public async ValueTask DisposeAsync()
        {
            blockingCollection.CompleteAdding();
            await mainTask;
            blockingCollection.Dispose();
        }

        public Task<T> ExecuteAsync<T>(Func<Task<T>> function)
        {
            var item = new QueueItem<T>(function);
            blockingCollection.Add(item);

            return item.Completion;
        }
    }
}
