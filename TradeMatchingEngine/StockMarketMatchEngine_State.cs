using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public virtual void Close()
            {
                throw new NotImplementedException();
            }

            public virtual void Enqueue(int price, int amount, Side side)
            {
                throw new NotImplementedException();
            }

            public virtual void Open()
            {
                throw new NotImplementedException();
            }

            public virtual void PreOpen()
            {
                throw new NotImplementedException();
            }
        }
        class Closed : StockMarketState
        {
            public Closed(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public void PreOpen(StockMarketMatchEngine stockMarketMatchEngine)
            {
                stockMarketMatchEngine.state = new PreOpened(StockMarketMatchEngine);
            }
        }
        class Opened : StockMarketState
        {
            public Opened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void Enqueue(int price, int amount, Side side)
            {
                StockMarketMatchEngine.enqueue(price, amount, side);
            }

        }
        class PreOpened : StockMarketState
        {
            public PreOpened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void Open()
            {
                StockMarketMatchEngine.open();
            }

            public override void Close()
            {
                StockMarketMatchEngine.close();
            }

            public override void Enqueue(int price, int amount, Side side)
            {
                StockMarketMatchEngine.enqueue(price, amount, side);
            }
        }
    }
}
