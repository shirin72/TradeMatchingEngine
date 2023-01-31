namespace TradeMatchingEngine
{
    public class StockMarketBlockingQueueDecorator : StockMarketMatchEngine
    {
        private readonly BlockingQueue queue;
        public StockMarketBlockingQueueDecorator(List<Order>? orders = null, long lastOrderId = 0, long lastTradeId = 0) : base(orders, lastOrderId, lastTradeId)
        {
            queue = new BlockingQueue();
        }


        private  async Task<T?> executeAsync<T>(Func<Task<T>> function, StockMarketEvents? events = null)
        {
            return await queue.ExecuteAsync(async () =>
            {
                setupEvents(events);
                try
                {
                    return await function();
                }
                finally
                {
                    clearEvents();
                }
            });
        }
        public virtual async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
        {
            return await executeAsync(async () => await cancelOrderAsync(orderId, events));
        }
        public virtual async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
        {
            return await executeAsync(async () => await modifieOrder(orderId, price, amount, expirationDate,events));
        }

        public virtual async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
        {
            return await executeAsync(async () => await processOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId), events);
        }

        public virtual async Task<long> PreProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, StockMarketEvents? events = null)
        {
            return await executeAsync(async () => await preProcessOrderAsync(price, amount, side, expireTime, fillAndKill));
        }

        private void setupEvents(StockMarketEvents? events)
        {
            if (events != null)
            {
                onOrderCreated = events.OnOrderCreated;
                onOrderModified = events.OnOrderModified;
                onTradeCreated = events.OnTradeCreated;
            }
        }

        private void clearEvents()
        {
            onOrderCreated = null;
            onOrderModified = null;
            onTradeCreated = null;
        }

        public async ValueTask DisposeAsync()
        {
            await queue.DisposeAsync();
        }

    }
}
