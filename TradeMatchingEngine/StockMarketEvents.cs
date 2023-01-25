using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public class StockMarketEvents
    {
        public Func<StockMarketMatchEngine, Order, Task> OnOrderCreated { get; set; }
        public Func<StockMarketMatchEngine, Order, Task> OnOrderModified { get; set; }
        public Func<StockMarketMatchEngine, Trade, Task> OnTradeCreated { get; set; }
    }
}
