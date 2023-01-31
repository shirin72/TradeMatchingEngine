﻿namespace TradeMatchingEngine
{
    public class StockMarketMatchEngine 
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> sellOrderQueue, buyOrderQueue;
        private readonly Queue<Order> preOrderQueue;
        private Order _lastOrder;
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
        private async Task<Order> createOrderRequest(int price, int amount, Side side, DateTime? expireTime, bool? fillAndKill, long? OrderParentId = null)
        {
            var order = new Order(id: setId(), side: side, price: price, amount: amount, expireTime: expireTime ?? DateTime.MaxValue, isFillAndKill: fillAndKill, OrderParentId);

            if (onOrderCreated != null)
                await onOrderCreated(this, order);

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

        protected async Task<long> preProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketEvents? events = null)
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

        protected async Task<long> processOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? OrderParentId = null, StockMarketEvents? events = null)
        {
            var order = await createOrderRequest(price, amount, side, expireTime, fillAndKill, OrderParentId);
            return await processOrder(order);
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

        protected async Task<long?> cancelOrderAsync(long orderId, StockMarketEvents? events)
        {
            var findOrder = allOrders.Where(a => a.Id == orderId).SingleOrDefault();
            findOrder?.SetStateCancelled();

            if (onOrderModified != null)
                await onOrderModified(this, findOrder);

            return findOrder?.Id;

        }

        protected async Task<long> modifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events)
        {
            await cancelOrderAsync(orderId, events);

            var orderSide = allOrders.Where(o => o.Id == orderId).Single().Side;
            long id = await processOrderAsync(price, amount, orderSide, expirationDate);

            return id;
        }

        #endregion
    }
}
