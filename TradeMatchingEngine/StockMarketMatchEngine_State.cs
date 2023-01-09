namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine
    {
        class StockMarketState : IStockMarketMatchEngine
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

            public virtual async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime=null)
            {
                throw new NotImplementedException();
            }

            public virtual void ClearQueue()
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

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime=null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side,  expireTime);
            }

            //public override void ClearQueue()
            //{
            //    StockMarketMatchEngine.clearQueue();
            //}
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

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime=null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side, expireTime);
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

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime=null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side,  expireTime);
            }

            //public override void ClearQueue()
            //{
            //    StockMarketMatchEngine.clearQueue();
            //}
        }
    }
}
