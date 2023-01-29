namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine
    {
        class StockMarketState :IStockMarketMatchEngine
        {
            public MarcketState Code;

            protected StockMarketMatchEngine StockMarketMatchEngine;

            public StockMarketState(StockMarketMatchEngine stockMarketMatchEngine)
            {
                this.StockMarketMatchEngine = stockMarketMatchEngine;
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

            ValueTask IAsyncDisposable.DisposeAsync()
            {
                throw new NotImplementedException();
            }
        }
        class Closed : StockMarketState
        {
            public Closed(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void PreOpen()
            {
                StockMarketMatchEngine.state = new PreOpened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.PreOpen;

               
            }
        }
        class Opened : StockMarketState
        {
            public Opened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void PreOpen()
            {
                StockMarketMatchEngine.state = new PreOpened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.PreOpen;
            }

            public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId,events);
            }

            public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.cancelOrderAsync(orderId, events);
            }
            public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.modifieOrder(orderId, price, amount, expirationDate, events);
            }
        }
        class PreOpened : StockMarketState
        {
            public PreOpened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void Close()
            {
                StockMarketMatchEngine.state = new Closed(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.Close;

            }

            public override void Open()
            {
                StockMarketMatchEngine.state = new Opened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.Open;

                
            }

            public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.preProcessOrderAsync(price, amount, side, expireTime, fillAndKill,orderParentId, events);
            }

            public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.cancelOrderAsync(orderId, events);
            }

            public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
            {
                return await StockMarketMatchEngine.modifieOrder(orderId, price, amount, expirationDate, events);
            }
        }
    }
}
