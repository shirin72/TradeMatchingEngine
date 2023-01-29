namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine : IStockMarketMatchEngine
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> sellOrderQueue, buyOrderQueue;
        private readonly Queue<Order> preOrderQueue;
        private StockMarketState state;
        private readonly BlockingQueue queue;
        private Order _lastOrder;
        private long _lastOrderId;
        private long _lastTradeId;
        #endregion

        #region PublicProperties
        public MarcketState State => state.Code;
        private Func<StockMarketMatchEngine, Order, Task> onOrderModified;
        private Func<StockMarketMatchEngine, Trade, Task> onTradeCreated;
        private Func<StockMarketMatchEngine, Order, Task> onOrderCreated;
        private bool isInitialized = false;
        public int TradeCount => tradeCount;
        public Order LastOrder => _lastOrder;

        public Func<StockMarketMatchEngine, Order, Task> FuncOrderModified { get { return this.onOrderModified; } }
        public Func<StockMarketMatchEngine, Trade, Task> FuncTradeCreated { get { return this.onTradeCreated; } }
        public Func<StockMarketMatchEngine, Order, Task> FuncOrderCreated { get { return this.onOrderCreated; } }
        #endregion

        public StockMarketMatchEngine(List<Order>? orders = null, long lastOrderId = 0, long lastTradeId = 0)
        {
            this.sellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.buyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());

            preOrderQueue = new Queue<Order>();
            state = new Closed(this);
            queue = new BlockingQueue();
            _lastOrderId = lastOrderId;
            _lastTradeId = lastTradeId;
            allOrders = new List<Order>();

            foreach (var order in orders ?? new List<Order>())
            {
                processOrder(order).GetAwaiter().GetResult();
            }
        }

        #region Properties
        private List<Order> allOrders;
        private List<Trade> trades = new();
        private int tradeCount;
        #endregion

        #region Public Method
        public IEnumerable<Order> AllOrders => allOrders;
        public int AllOrdersCount() => allOrders.Count;
        public IEnumerable<Trade> Trade => trades;
        public int AllTradeCount() => trades.Count;
        public PriorityQueue<Order, Order> GetSellOrderQueue()
        {
            return sellOrderQueue;
        }
        public Queue<Order> GetPreOrderQueue()
        {
            return preOrderQueue;
        }
        public int GetPreOrderQueueCount()
        {
            return preOrderQueue.Count;
        }
        public PriorityQueue<Order, Order> GetBuyOrderQueue()
        {
            return buyOrderQueue;
        }
        public int GetBuyOrderCount()
        {
            return buyOrderQueue.Count;
        }
        public int GetSellOrderCount()
        {
            return sellOrderQueue.Count;
        }

        public void PreOpen()
        {
            state.PreOpen();
        }
        public void Open()
        {
            state.Open();
        }
        public void Close()
        {
            state.Close();
        }
        public IEnumerable<Trade> GetAllTradeByOrderId(int orderId)
        {
            return trades.Where(x => x.Id == orderId).ToList();
        }
        public async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
        {
            return await queue.ExecuteAsync(async () => await state.CancelOrderAsync(orderId, events));
        }
        public async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents events = null)
        {
            return await queue.ExecuteAsync(async () => await state.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId, events));
        }
        public async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents events = null)
        {
            return await queue.ExecuteAsync(async () => await state.ModifieOrder(orderId, price, amount, expirationDate ?? DateTime.MaxValue, events));
        }
        #endregion

        #region Private Method
        private async Task<Order> createOrderRequest(int price, int amount, Side side, DateTime? expireTime, bool? fillAndKill, long? OrderParentId = null)
        {
            var order = new Order(id: SetId(), side: side, price: price, amount: amount, expireTime: expireTime ?? DateTime.MaxValue, isFillAndKill: fillAndKill, OrderParentId);

            if (onOrderCreated != null)
                await onOrderCreated(this, order);

            return order;
        }

        private long SetId()
        {
            if (_lastOrderId == 0)
            {
                _lastOrderId = 1;

                return _lastOrderId;
            }

            return Interlocked.Increment(ref _lastOrderId);
        }

        private async Task<long> preProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketEvents? events = null)
        {
            setupEvents(events);
            try
            {
                var preOrder = await createOrderRequest(price, amount, side, expireTime, fillAndKill, OrderParentId);

                allOrders.Add(preOrder);

                if (preOrder.Side == Side.Sell)
                {
                    this.sellOrderQueue.Enqueue(preOrder, preOrder);
                    return preOrder.Id;
                }

                preOrderQueue.Enqueue(preOrder);

                return preOrder.Id;
            }
            finally
            {
                clearEvents();
            }
        }

        private async Task<long> processOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketEvents? events = null)
        {
            setupEvents(events);

            try
            {
                var order = await createOrderRequest(price, amount, side, expireTime, fillAndKill, OrderParentId);
                return await processOrder(order);
            }
            finally
            {
                clearEvents();
            }
        }

        private async Task<long> processOrder(Order order)
        {
            PriorityQueue<Order, Order> ordersQueue, otherSideOrdersQueue;

            Func<bool> priceCheck;

            initializeQueue();
            allOrders.Add(order);
            initiateTheQueueSideAndPriceCheck();

            while (order.Amount > 0 && otherSideOrdersQueue.Count > 0 && priceCheck())
            {
                var peekedOrder = otherSideOrdersQueue.Peek();
                CheckForNotCanceled(peekedOrder);
                if (peekedOrder.IsExpired || peekedOrder.ExpireTime < DateTime.Now || peekedOrder.GetOrderState() == OrderState.Cancell)
                {
                    allOrders.Remove(peekedOrder);
                    otherSideOrdersQueue.Dequeue();
                    continue;
                }

                tradeCount++;
                await makeTrade(order, peekedOrder);

                if (peekedOrder.HasCompleted)
                {
                    otherSideOrdersQueue.Dequeue();
                    allOrders.Remove(peekedOrder);

                    if (order.Amount <= 0)
                    {
                        allOrders.Remove(order);
                    }

                    continue;
                }
            }
            var isFillAndKill = order.IsFillAndKill ?? false;

            if (order.Amount > 0 && !isFillAndKill)
            {
                ordersQueue.Enqueue(order, order);

                return order.Id;
            }

            if (order.Amount <= 0 || isFillAndKill)
                allOrders.Remove(order);


            return order.Id;


            void initiateTheQueueSideAndPriceCheck()
            {
                if (order.Side == Side.Sell)
                {
                    ordersQueue = sellOrderQueue;
                    otherSideOrdersQueue = buyOrderQueue;
                    priceCheck = () => order.Price <= otherSideOrdersQueue.Peek().Price;
                    return;
                }

                ordersQueue = buyOrderQueue;
                otherSideOrdersQueue = sellOrderQueue;
                priceCheck = () => order.Price >= otherSideOrdersQueue.Peek().Price;
            }

            void initializeQueue()
            {
                if (!isInitialized)
                {
                    foreach (var item in allOrders)
                    {
                        if (item.GetOrderState() == OrderState.Cancell)
                        {
                            continue;
                        }

                        if (item.Side == Side.Sell)
                        {
                            sellOrderQueue.Enqueue(item, item);
                            continue;
                        }

                        buyOrderQueue.Enqueue(item, item);
                    }
                }

                isInitialized = true;
            }

            async Task makeTrade(Order order, Order otherSideOrder)
            {
                var amount = otherSideOrder.Amount > order.Amount ? order.Amount : otherSideOrder.Amount;

                var tradeItem = new Trade(
                    id: Interlocked.Increment(ref _lastTradeId),
                    buyOrderId: order.Side == Side.Buy ? order.Id : otherSideOrder.Id,
                    sellOrderId: order.Side == Side.Sell ? order.Id : otherSideOrder.Id,
                    amount: amount,
                    price: order.Side == Side.Sell ? order.Price : otherSideOrder.Price
                    );

                int currentOrderAmount = order.Amount;
                order.DecreaseAmount(otherSideOrder.Amount);

                if (onOrderModified != null)
                    await onOrderModified(this, order);

                otherSideOrder.DecreaseAmount(currentOrderAmount);

                if (onOrderModified != null)
                    await onOrderModified(this, otherSideOrder);

                trades.Add(tradeItem);

                if (onTradeCreated != null)
                    await onTradeCreated(this, tradeItem);
            }

            bool CheckForNotCanceled(Order order)
            {
                var checkedOrder = allOrders.Where(o => o.Id == order.Id).Single();

                if (checkedOrder.GetOrderState() == OrderState.Cancell)
                {
                    order.SetStateCancelled();

                    return true;
                }

                return false;
            }
        }

        private async Task<long?> cancelOrderAsync(long orderId, StockMarketEvents? events)
        {
            setupEvents(events);

            try
            {
                var findOrder = allOrders.Where(a => a.Id == orderId).SingleOrDefault();
                findOrder?.SetStateCancelled();

                if (onOrderModified != null)
                    await onOrderModified(this, findOrder);

                return findOrder?.Id;

            }
            finally
            {
                clearEvents();
            }
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
        private async Task<long> modifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events)
        {
            setupEvents(events);
            long id;
            try
            {
                await cancelOrderAsync(orderId, events);

                var orderSide = allOrders.Where(o => o.Id == orderId).Single().Side;
                id = await state.ProcessOrderAsync(price, amount, orderSide, expirationDate, orderParentId: orderId);
            }
            finally
            {
                clearEvents();
            }

            return id;
        }

        public async ValueTask DisposeAsync()
        {
            await queue.DisposeAsync();
        }
        #endregion
    }
}
