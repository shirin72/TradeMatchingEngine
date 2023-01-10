using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    internal class StockMarketMatchEngineEvents:EventArgs
    {
        public EventType eventType { get; set; }

        public object? EventObject { get; set; }
        public string Description { get; set; }
    }
}
