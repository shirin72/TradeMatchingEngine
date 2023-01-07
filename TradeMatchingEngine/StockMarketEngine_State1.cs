using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine
    {
        public class StockMarketEngine_State1 : IStockMarketMatchEngine
        {
            protected StockMarketMatchEngine StockMarketEngine;
            public StockMarketEngine_State1(StockMarketMatchEngine stockMarketEngine)
            {
                this.StockMarketEngine = stockMarketEngine;
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

        public class Close1 : StockMarketEngine_State1
        {
            public Close1(StockMarketMatchEngine stockMarketEngine) : base(stockMarketEngine)
            {
            }

            public override void PreOpen()
            {
                StockMarketEngine.stockMarketEngine_State1 = new PreOpen1(StockMarketEngine);
            }
        }

        public class Open1 : StockMarketEngine_State1
        {
            public Open1(StockMarketMatchEngine stockMarketEngine) : base(stockMarketEngine)
            {
            }

            public override void Open()
            {
                StockMarketEngine.open();
            }
        }

        public class PreOpen1 : StockMarketEngine_State1
        {
            public PreOpen1(StockMarketMatchEngine stockMarketEngine) : base(stockMarketEngine)
            {
            }

            public override void Open()
            {
                StockMarketEngine.preOpen();
            }

            public override void Close()
            {
                StockMarketEngine.stockMarketEngine_State1 = new Close1(StockMarketEngine);
            }
        }
    }
}
