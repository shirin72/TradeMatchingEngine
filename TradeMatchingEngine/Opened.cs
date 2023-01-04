using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public class Opened : IStockMarketMatchEngine
    {
        public void Close(StockMarketMatchEngine stockMarketMatchEngine)
        {
            throw new Exception("The Market Is Opened So It cannot be Closed");
        }

        public void Open(StockMarketMatchEngine stockMarketMatchEngine)
        {
           
        }

        public void PreOpen(StockMarketMatchEngine stockMarketMatchEngine)
        {
            stockMarketMatchEngine.StockMarketMatchEngineState = new PreOpened();
        }
    }
}
