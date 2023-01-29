namespace TradeMatchingEngine
{
    public class StockMarketMatchEngineStateProxy: StockMarketMatchEngine,IStockMarketMatchEngineWithState
    {
        private StockMarketState state;
        private IStockMarketMatchEngine stockMarketMatchEngine;

        public StockMarketMatchEngineStateProxy(IStockMarketMatchEngine stockMarketMatchEngine)
        {
            this.stockMarketMatchEngine = stockMarketMatchEngine;
            state = new Closed(this);
        }

        public MarcketState State => state.Code;

        public override Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
        {
            return state.CancelOrderAsync(orderId,events);
        }

        public void Close()
        {
            state.Close();
        }

        public override Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
        {
            return state.ModifieOrder(orderId, price, amount, expirationDate, events);
        }

        public void Open()
        {
            state.Open();
        }

        public void PreOpen()
        {
            state.PreOpen();
        }

        public Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
        {
            return state.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId, events);
        }

        class StockMarketState :IStockMarketMatchEngine
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

            public virtual async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
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
                Code=MarcketState.Open;
            }

            public override void PreOpen()
            {
                StockMarketMatchEngineProxy.state = new PreOpened(StockMarketMatchEngineProxy);
            }

            public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId,events);
            }

            public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.CancelOrderAsync(orderId, events);
            }
            public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.ModifieOrder(orderId, price, amount, expirationDate, events);
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

            public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill,orderParentId, events);
            }

            public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.CancelOrderAsync(orderId, events);
            }

            public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngineProxy.stockMarketMatchEngine.ModifieOrder(orderId, price, amount, expirationDate, events);
            }
        }
    }
}
