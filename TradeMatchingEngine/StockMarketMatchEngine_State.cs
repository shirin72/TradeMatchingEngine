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

            public virtual void Close ()
            {
                throw new NotImplementedException();
            }

            public virtual void Enqueue(int price, int amount, Side side)
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

            public override void Enqueue(int price, int amount, Side side)
            {
                StockMarketMatchEngine.enqueue(price, amount, side);
            }

            public override void ClearQueue()
            {
                StockMarketMatchEngine.clearQueue();
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

            public override void Enqueue(int price, int amount, Side side)
            {
                StockMarketMatchEngine.enqueueOrder(price, amount, side);
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

            public override void Enqueue(int price, int amount, Side side)
            {
                StockMarketMatchEngine.enqueue(price, amount, side);
            }

            public override void ClearQueue()
            {
                StockMarketMatchEngine.clearQueue();
            }
        }
    }
}
