using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine
    {
        class StockMarketMatchState: IStockMarketEngine
        {
            protected readonly StockMarketMatchEngine stockMarketMatchEngine;

            public StockMarketMatchState(StockMarketMatchEngine stockMarketMatchEngine)
            {
                this.stockMarketMatchEngine = stockMarketMatchEngine;
            }

            public virtual void Close()
            {
                throw new NotImplementedException();
            }

            public virtual void Open()
            {
                throw new NotImplementedException();
            }
        }
        class OpenState : StockMarketMatchState
        {
            public OpenState(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }
            public override void Open()
            {
               
            }
            public override void Close()
            {
                stockMarketMatchEngine.close();
            }
        }
        class CloseState : StockMarketMatchState
        {
            public CloseState(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }
            public override void Open()
            {
                stockMarketMatchEngine.open();
            }
            public override void Close()
            {
                
            }
        }

        private void open()
        {
            throw new NotImplementedException();
        }
    }
}
