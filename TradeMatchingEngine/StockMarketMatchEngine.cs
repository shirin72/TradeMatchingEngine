namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine : IStockMarketMatchEngine
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> SellOrderQueue, BuyOrderQueue;
        private readonly Queue<Order> preOrderQueue;
        private StockMarketState state;
        public List<TradeInfo> TradeInfo = new();
        public MarcketState State => state.Code;
        public delegate void Notify(Object sender, EventArgs e);
        public event Notify ProcessCompleted;
        #endregion

        public StockMarketMatchEngine()
        {
            this.SellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.BuyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());
            this.AllOrders = new List<Order>();
            preOrderQueue = new Queue<Order>();
            state = new Closed(this);
        }

        #region Properties
        public List<Order> AllOrders { get; set; }
        public int TradeCount { get; set; }
        #endregion

        #region Public Method
        public PriorityQueue<Order, Order> GetSellOrderQueue()
        {
            return SellOrderQueue;
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
            return BuyOrderQueue;
        }
        public int GetBuyOrderCount()
        {
            return BuyOrderQueue.Count;
        }
        public int GetSellOrderCount()
        {
            return SellOrderQueue.Count;
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
        public List<TradeInfo> GetAllTradeInfo()
        {
            return this.TradeInfo.ToList();
        }
        public List<TradeInfo> GetAllTradeByOrderId(int orderId)
        {
            return this.TradeInfo.Where(x => x.OwnerId == orderId).ToList();
        }
        public async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null)
        {
            return await state.ProcessOrderAsync(price, amount, side, expireTime);
        }
        #endregion

        #region Private Method
        private Order CreateOrderRequest(int price, int amount, Side side, DateTime? expireTime = null)
        {
            return new Order() { Amount = amount, Side = side, Price = price, Id = SetId(), ExpireTime = expireTime ?? DateTime.MaxValue };
        }

        private int SetId()
        {
            int id;
            if (AllOrders.Count == 0)
            {
                id = 1;

                return id;
            }
            var findMaxId = AllOrders.Max(x => x.Id);

            return Interlocked.Increment(ref findMaxId);
        }

        private async Task<int> processOrderAsync(int price, int amount, Side side, DateTime? expireTime = null)
        {
            var order = CreateOrderRequest(price, amount, side, expireTime);

            PriorityQueue<Order, Order> ordersQueue, otherSideOrdersQueue;

            Func<bool> priceCheck;

            switch (this.State)
            {
                case MarcketState.Open:
                    AllOrders.Add(order);
                    initiateTheQueueSideAndPriceCheck();

                    while (order.Amount > 0 && otherSideOrdersQueue.Count > 0 && priceCheck())
                    {

                        var peekedOrder = otherSideOrdersQueue.Peek();

                        if (peekedOrder.IsExpired || peekedOrder.ExpireTime < DateTime.Now)
                        {
                            var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                            {
                                EventObject = peekedOrder,
                                eventType = EventType.OrderExpired,
                                Description = $"Order with Id: {peekedOrder.Id} Is Expired And Removed From Orders"

                            };
                            OnProcessCompleted(stockMarketMatchEngineEvents);

                            AllOrders.Remove(peekedOrder);
                            otherSideOrdersQueue.Dequeue();
                            continue;
                        }

                        TradeCount++;
                        await makeTrade(order, peekedOrder).ConfigureAwait(false);


                        if (peekedOrder.HasCompleted)
                        {
                            otherSideOrdersQueue.Dequeue();
                            AllOrders.Remove(peekedOrder);

                            if (order.Amount <= 0)
                            {
                                AllOrders.Remove(order);
                            }

                            continue;
                        }
                    }

                    if (order.Amount > 0 && order.IsFillAndKill != true)
                    {

                        ordersQueue.Enqueue(order, order);

                        var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                        {

                            eventType = EventType.OrderEnqued,
                            Description = $"Order With Id: {order.Id} Has been Enqueued",
                            EventObject = order,


                        };
                        OnProcessCompleted(stockMarketMatchEngineEvents);
                        return order.Id;
                    }


                    if (order.Amount <= 0)
                        AllOrders.Remove(order);

                    return order.Id;

                case MarcketState.PreOpen:

                    AllOrders.Add(order);

                    if (order.Side == Side.Sell)
                    {
                        this.SellOrderQueue.Enqueue(order, order);
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
                            ordersQueue = SellOrderQueue;
                            otherSideOrdersQueue = BuyOrderQueue;
                            priceCheck = () => order.Price <= otherSideOrdersQueue.Peek().Price;
                            return;
                        }

                        ordersQueue = BuyOrderQueue;
                        otherSideOrdersQueue = SellOrderQueue;
                        priceCheck = () => order.Price >= otherSideOrdersQueue.Peek().Price;
                    }
            }

            async Task makeTrade(Order order, Order otherSideOrder)
            {
                var amount = otherSideOrder.Amount <= order.Amount ? otherSideOrder.Amount : otherSideOrder.Amount - order.Amount;
                var trade = new TradeInfo()
                {
                    Amount = amount,
                    Price = order.Side == Side.Sell ? order.Price : otherSideOrder.Price,
                    OwnerId = order.Id,
                    BuyOrderId = order.Side == Side.Buy ? order.Id : otherSideOrder.Id,
                    SellOrderId = order.Side == Side.Sell ? order.Id : otherSideOrder.Id
                };

                int orderRemainingAmount = otherSideOrder.Amount > order.Amount ? 0 : order.Amount - otherSideOrder.Amount;

                otherSideOrder.Amount = otherSideOrder.Amount < order.Amount ? 0 : otherSideOrder.Amount - order.Amount;

                order.Amount = orderRemainingAmount;

                TradeInfo.Add(trade);

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventObject = trade,
                    eventType = EventType.TradeExecuted,
                    Description = $"Trade Has Been Executed For Order {trade.OwnerId}" +
                    $" with Price of {trade.Price} and Amount of {amount}"
                };

                OnProcessCompleted(stockMarketMatchEngineEvents);
            }
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
