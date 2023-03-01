namespace Domain
{
    public class StockMarketMatchEngineStateProxy : StockMarketBlockingQueueDecorator, IStockMarketMatchEngineWithState
    {
        private StockMarketState state;

        public StockMarketMatchEngineStateProxy(List<Order>? orders = null, long lastOrderId = 0, long lastTradeId = 0) : base(orders, lastOrderId, lastTradeId)
        {
            state = new Closed(this);
        }

        public MarcketState State => state.Code;

        public override Task<IStockMarketMatchingEngineProcessContext> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null)
        {
            return state.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId);
        }
        public override Task<IStockMarketMatchingEngineProcessContext> CancelOrderAsync(long orderId)
        {
            return state.CancelOrderAsync(orderId);
        }

        public override Task<IStockMarketMatchingEngineProcessContext> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
        {
            return state.ModifieOrder(orderId, price, amount, expirationDate);
        }

        public void Close()
        {
            state.Close();
        }
        public void Open()
        {
            state.Open();
        }

        public void PreOpen()
        {
            state.PreOpen();
        }


        private Task<IStockMarketMatchingEngineProcessContext> myProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null)
        {
            return base.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId);
        }
        private Task<IStockMarketMatchingEngineProcessContext> myCancelOrderAsync(long orderId)
        {
            return base.CancelOrderAsync(orderId);
        }
        private Task<IStockMarketMatchingEngineProcessContext> myModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
        {
            return base.ModifieOrder(orderId, price, amount, expirationDate);
        }

        private Task<IStockMarketMatchingEngineProcessContext> preModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
        {
            return base.PreModifieOrder(orderId, price, amount, expirationDate);
        }

        class StockMarketState : IStockMarketMatchEngine
        {
            public MarcketState Code { get; protected set; }

            protected StockMarketMatchEngineStateProxy StockMarketMatchEngineProxy;

            public StockMarketState(StockMarketMatchEngineStateProxy stockMarketMatchEngineProxy)
            {
                this.StockMarketMatchEngineProxy = stockMarketMatchEngineProxy;
            }

            public virtual void PreOpen()
            {
                throw new NotImplementedException();
            }

            public virtual void Open()
            {
                throw new NotImplementedException();
            }

            public virtual void Close()
            {
                throw new NotImplementedException();
            }

            public virtual async Task<IStockMarketMatchingEngineProcessContext> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<IStockMarketMatchingEngineProcessContext> CancelOrderAsync(long orderId)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<IStockMarketMatchingEngineProcessContext> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
            {
                throw new NotImplementedException();
            }

            async ValueTask IAsyncDisposable.DisposeAsync()
            {
                await StockMarketMatchEngineProxy.DisposeAsync();
            }

        }
        class Closed : StockMarketState
        {
            public Closed(StockMarketMatchEngineStateProxy stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
                Code = MarcketState.Close;
            }

            public override void PreOpen()
            {
                StockMarketMatchEngineProxy.state = new PreOpened(StockMarketMatchEngineProxy);
            }
        }
        class Opened : StockMarketState
        {
            public Opened(StockMarketMatchEngineStateProxy stockMarketMatchEngineProxy) : base(stockMarketMatchEngineProxy)
            {
                Code = MarcketState.Open;
            }

            public override void PreOpen()
            {
                StockMarketMatchEngineProxy.state = new PreOpened(StockMarketMatchEngineProxy);
            }

            public override async Task<IStockMarketMatchingEngineProcessContext> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null)
            {
                return await StockMarketMatchEngineProxy.myProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId);
            }

            public override async Task<IStockMarketMatchingEngineProcessContext> CancelOrderAsync(long orderId)
            {
                return await StockMarketMatchEngineProxy.myCancelOrderAsync(orderId);
            }

            public override async Task<IStockMarketMatchingEngineProcessContext> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
            {
                return await StockMarketMatchEngineProxy.myModifieOrder(orderId, price, amount, expirationDate);
            }
        }
        class PreOpened : StockMarketState
        {
            public PreOpened(StockMarketMatchEngineStateProxy stockMarketMatchEngineProxy) : base(stockMarketMatchEngineProxy)
            {
                Code = MarcketState.PreOpen;
            }

            public override void Close()
            {
                StockMarketMatchEngineProxy.state = new Closed(StockMarketMatchEngineProxy);
            }

            public override void Open()
            {
                StockMarketMatchEngineProxy.state = new Opened(StockMarketMatchEngineProxy);
            }

            public async override Task<IStockMarketMatchingEngineProcessContext> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null)
            {
                return  StockMarketMatchEngineProxy.preProcessOrderAsync(price, amount, side, expireTime, fillAndKill);
            }

            public override async Task<IStockMarketMatchingEngineProcessContext> CancelOrderAsync(long orderId)
            {
                return await StockMarketMatchEngineProxy.CancelOrderAsync(orderId);
            }

            public override async Task<IStockMarketMatchingEngineProcessContext> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate)
            {
                return await StockMarketMatchEngineProxy.preModifieOrder(orderId, price, amount, expirationDate);
            }
        }
    }
}
