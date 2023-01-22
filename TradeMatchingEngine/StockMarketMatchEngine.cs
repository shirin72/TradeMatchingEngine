using System.Collections.Concurrent;

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
        #endregion

        #region PublicProperties
        public MarcketState State => state.Code;
        public delegate void Notify(object sender, EventArgs e);
        public  event Notify ProcessCompleted;
        public int TradeCount => tradeCount;
        public Order LastOrder => _lastOrder;

        #endregion

        public StockMarketMatchEngine()
        {
            this.sellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.buyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());
            this.allOrders = new List<Order>();
            preOrderQueue = new Queue<Order>();
            state = new Closed(this);
            queue = new BlockingQueue();
        }

        #region Properties
        private List<Order> allOrders;
        private List<Trade> trade = new();
        private int tradeCount;
        #endregion

        #region Public Method
        public IEnumerable<Order> AllOrders => allOrders;
        public int AllOrdersCount() => allOrders.Count;
        public IEnumerable<ITrade> Trade => trade;
        public int AllTradeCount() => trade.Count;
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
        public void ClearQueue()
        {
            state.ClearQueue();
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
        public IEnumerable<ITrade> GetAllTradeByOrderId(int orderId)
        {
            return trade.Where(x => x.OwnerId == orderId).ToList();
        }
        public async Task<int> CancelOrder(int orderId)
        {
            return await queue.ExecuteAsync(async () => await state.CancellOrderAsync(orderId));
        }
        public async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null,int ? orderParentId = null)
        {
            return await queue.ExecuteAsync(async () => await state.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId));
        }
        public async Task<int> ModifieOrder(int orderId, int price, int amount, DateTime expirationDate)
        {
            return await queue.ExecuteAsync(async () => await state.ModifieOrder(orderId, price, amount, expirationDate));
        }
        #endregion

        #region Private Method
        private Order CreateOrderRequest(int price, int amount, Side side, DateTime? expireTime, bool? fillAndKill, int? OrderParentId = null)
        {
            var order = new Order(id: SetId(), side: side, price: price, amount: amount, expireTime: expireTime ?? DateTime.MaxValue, fillAndKill,OrderParentId);

            var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
            {
                EventObject = order,
                EventType = EventType.OrderCreated,
                Description = $"Order with Id: {order.Id} Is Created"
            };

            OnProcessCompleted(stockMarketMatchEngineEvents);
            _lastOrder=order;
            return order;
        }

        private int SetId()
        {
            int id;
            if (AllOrdersCount() == 0)
            {
                id = 1;

                return id;
            }
            var findMaxId = allOrders.Max(x => x.Id);

            return Interlocked.Increment(ref findMaxId);
        }
        private async Task<int> processOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null,int ? OrderParentId=null)
        {
            var order = CreateOrderRequest(price, amount, side, expireTime, fillAndKill, OrderParentId);

            PriorityQueue<Order, Order> ordersQueue, otherSideOrdersQueue;

            Func<bool> priceCheck;

            switch (this.State)
            {
                case MarcketState.Open:
                    allOrders.Add(order);
                    initiateTheQueueSideAndPriceCheck();

                    while (order.Amount > 0 && otherSideOrdersQueue.Count > 0 && priceCheck())
                    {
                        var peekedOrder = otherSideOrdersQueue.Peek();
                        CheckForNotCanceled(peekedOrder);
                        if (peekedOrder.IsExpired || peekedOrder.ExpireTime < DateTime.Now || peekedOrder.GetOrderState() == OrderState.Cancell)
                        {
                            var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                            {
                                EventObject = peekedOrder,
                                EventType = EventType.OrderExpired,
                                Description = $"Order with Id: {peekedOrder.Id} Is Expired And Removed From Orders"
                            };
                            OnProcessCompleted(stockMarketMatchEngineEvents);

                            allOrders.Remove(peekedOrder);
                            otherSideOrdersQueue.Dequeue();
                            continue;
                        }

                        tradeCount++;
                        await makeTrade(order, peekedOrder).ConfigureAwait(false);

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

                    if (order.Amount > 0 && !order.IsFillAndKill.HasValue)
                    {
                        ordersQueue.Enqueue(order, order);

                        var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                        {
                            EventType = EventType.OrderEnqued,
                            Description = $"Order With Id: {order.Id} Has been Enqueued",
                            EventObject = order,
                        };

                        OnProcessCompleted(stockMarketMatchEngineEvents);

                        return order.Id;
                    }

                    if (order.Amount <= 0 || order.IsFillAndKill.HasValue)
                        allOrders.Remove(order);

                    return order.Id;

                case MarcketState.PreOpen:

                    allOrders.Add(order);

                    if (order.Side == Side.Sell)
                    {
                        this.sellOrderQueue.Enqueue(order, order);
                        return order.Id;
                    }

                    preOrderQueue.Enqueue(order);

                    return order.Id;

                case MarcketState.Close:
                    throw new Exception("Market is Close!");

                default:
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
            }

            async Task makeTrade(Order order, Order otherSideOrder)
            {
                var amount = otherSideOrder.Amount > order.Amount ? order.Amount : otherSideOrder.Amount;

                var tradeItem = new Trade(
                    tradeId: DateTime.Now.Ticks,
                    ownerId: order.Id,
                    buyOrderId: order.Side == Side.Buy ? order.Id : otherSideOrder.Id,
                    sellOrderId: order.Side == Side.Sell ? order.Id : otherSideOrder.Id,
                    amount: amount,
                    price: order.Side == Side.Sell ? order.Price : otherSideOrder.Price
                    );

                int currentOrderAmount = order.Amount;
                order.DecreaseAmount(otherSideOrder.Amount);
                otherSideOrder.DecreaseAmount(currentOrderAmount);

                trade.Add(tradeItem);

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventObject = trade,
                    EventType = EventType.TradeExecuted,
                    Description = $"Trade Has Been Executed For Order {tradeItem.OwnerId}" +
                    $" with Price of {tradeItem.Price} and Amount of {amount}"
                };

                OnProcessCompleted(stockMarketMatchEngineEvents);
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

        private int cancellOrderAsync(int orderId)
        {
            var findOrder = allOrders.Where(a => a.Id == orderId).Single();

            findOrder.SetStateCancelled();

            var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
            {
                EventObject = findOrder,
                EventType = EventType.OrderCancelled,
                Description = $"Order with Id: {findOrder.Id} Is OrderCancelled From Orders"
            };

            OnProcessCompleted(stockMarketMatchEngineEvents);

            return findOrder.Id;
        }

        private async Task<int> modifieOrder(int orderId, int price, int amount, DateTime expirationDate)
        {
            cancellOrderAsync(orderId);

            var orderSide = allOrders.Where(o => o.Id == orderId).Single().Side;

            return await state.ProcessOrderAsync(price, amount, orderSide, expirationDate, orderParentId: orderId);
        }

        #endregion

        #region ProtectedMethod
        protected virtual void OnProcessCompleted(EventArgs eventArgs)
        {
            var result = eventArgs as StockMarketMatchEngineEvents;
            ProcessCompleted?.Invoke(this, result);
        }
        #endregion
    }
}
