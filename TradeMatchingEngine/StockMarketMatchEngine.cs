namespace Domain
{
    public class StockMarketMatchEngine
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> sellOrderQueue, buyOrderQueue;
        private readonly Queue<Order> preOrderQueue;
        private long _lastOrderId;
        private long _lastTradeId;
        #endregion

        #region PublicProperties
        protected Func<StockMarketMatchEngine, Order, Task> onOrderModified;
        protected Func<StockMarketMatchEngine, Trade, Task> onTradeCreated;
        protected Func<StockMarketMatchEngine, Order, Task> onOrderCreated;
        private bool isInitialized = false;
        public int TradeCount => tradeCount;
        #endregion

        public StockMarketMatchEngine(List<Order>? orders = null, long lastOrderId = 0, long lastTradeId = 0)
        {
            this.sellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.buyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());

            preOrderQueue = new Queue<Order>();

            _lastOrderId = lastOrderId;
            _lastTradeId = lastTradeId;

            allOrders = new List<Order>();

            foreach (var order in orders ?? new List<Order>())
            {
                processOrder(new StockMarketMatchingEngineProcessContext(), order);
            }
        }

        #region Properties
        private List<Order> allOrders;
        private List<Trade> trades = new();
        private int tradeCount;
        #endregion

        #region Public Method
        public IEnumerable<IOrder> AllOrders => allOrders;
        public int AllOrdersCount() => allOrders.Count;
        public IEnumerable<ITrade> Trade => trades;
        public int AllTradeCount() => trades.Count;
        public Queue<Order> GetPreOrderQueue()
        {
            return preOrderQueue;
        }
        public int GetPreOrderQueueCount()
        {
            return preOrderQueue.Count;
        }
        public int GetBuyOrderCount()
        {
            return buyOrderQueue.Count;
        }
        public int GetSellOrderCount()
        {
            return sellOrderQueue.Count;
        }
        #endregion

        #region Private Method
        private Order createOrderRequest(StockMarketMatchingEngineProcessContext processContext, int price, int amount, Side side, DateTime? expireTime, bool? fillAndKill, long? OrderParentId = null)
        {
            var order = new Order(id: setId(), side: side, price: price, amount: amount, expireTime: expireTime ?? DateTime.MaxValue, orderState: OrderStates.Register,isFillAndKill: fillAndKill, orderParentId: OrderParentId);

            processContext.OrderCreated(order);

            return order;
        }

        private long setId()
        {
            if (_lastOrderId == 0)
            {
                _lastOrderId = 1;

                return _lastOrderId;
            }

            return Interlocked.Increment(ref _lastOrderId);
        }

        protected IStockMarketMatchingEngineProcessContext preProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketMatchingEngineProcessContext? processContext = null)
        {
            processContext = processContext ?? new StockMarketMatchingEngineProcessContext();
            var preOrder = createOrderRequest(processContext, price, amount, side, expireTime, fillAndKill, OrderParentId);

            allOrders.Add(preOrder);

            if (preOrder.Side == Side.Sell)
            {
                this.sellOrderQueue.Enqueue(preOrder, preOrder);
                return processContext;
            }

            preOrderQueue.Enqueue(preOrder);

            return processContext;
        }

        protected IStockMarketMatchingEngineProcessContext processOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketMatchingEngineProcessContext? processContext = null)
        {
            processContext = processContext ?? new StockMarketMatchingEngineProcessContext();
            var order = createOrderRequest(processContext, price, amount, side, expireTime, fillAndKill, OrderParentId);
            return processOrder(processContext, order);
        }

        private IStockMarketMatchingEngineProcessContext processOrder(StockMarketMatchingEngineProcessContext processContext, Order order)
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
                if (peekedOrder.IsExpired || peekedOrder.ExpireTime < DateTime.Now || peekedOrder.GetOrderState() == OrderStates.Cancell)
                {
                    allOrders.Remove(peekedOrder);
                    otherSideOrdersQueue.Dequeue();
                    continue;
                }

                tradeCount++;
                makeTrade(order, peekedOrder);

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

                return processContext;
            }

            if (order.Amount <= 0 || isFillAndKill)
                allOrders.Remove(order);


            return processContext;


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
                        if (item.GetOrderState() == OrderStates.Cancell)
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

            void makeTrade(Order order, Order otherSideOrder)
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

                processContext.OrderModified(order.Clone((int)order.OriginalAmount));

                //if (onOrderModified != null)
                //    await onOrderModified(this, order);

                otherSideOrder.DecreaseAmount(currentOrderAmount);

                //if (onOrderModified != null)
                //    await onOrderModified(this, otherSideOrder);

                processContext.OrderModified(otherSideOrder.Clone((int)otherSideOrder.OriginalAmount));

                trades.Add(tradeItem);
                processContext.TradeCreated(tradeItem);

                //if (onTradeCreated != null)
                //    await onTradeCreated(this, tradeItem);
            }

            bool CheckForNotCanceled(Order order)
            {
                var checkedOrder = allOrders.Where(o => o.Id == order.Id).Single();

                if (checkedOrder.GetOrderState() == OrderStates.Cancell)
                {
                    order.SetStateCancelled();

                    return true;
                }

                return false;
            }
        }

        protected IStockMarketMatchingEngineProcessContext cancelOrderAsync(long orderId, StockMarketMatchingEngineProcessContext? processContext = null)
        {
            processContext = processContext ?? new StockMarketMatchingEngineProcessContext();

            var findOrder = allOrders.Where(a => a.Id == orderId).SingleOrDefault();

            if (findOrder != null)
            {
                findOrder?.SetStateCancelled();

                processContext.OrderModified(findOrder);

                //if (onOrderModified != null)
                //    await onOrderModified(this, findOrder);

                return processContext;
            }

            throw new Exception(message: "Order Has Not Been Defined");
        }

        protected IStockMarketMatchingEngineProcessContext modifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
        {
            var processContext = new StockMarketMatchingEngineProcessContext();

            cancelOrderAsync(orderId, processContext);

            var orderSide = allOrders.Where(o => o.Id == orderId).Single().Side;

            processOrderAsync(price, amount, orderSide, expirationDate, processContext: processContext);

            return processContext;
        }

        protected IStockMarketMatchingEngineProcessContext preModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
        {
            var processContext = new StockMarketMatchingEngineProcessContext();

            var cancelledOrder= cancelOrderAsync(orderId, processContext);

            var orderSide = allOrders.Where(o => o.Id == orderId).Single().Side;

            preProcessOrderAsync(price, amount, orderSide, expirationDate,processContext: cancelledOrder as StockMarketMatchingEngineProcessContext);

            return processContext;
        }

        #endregion
    }
}
