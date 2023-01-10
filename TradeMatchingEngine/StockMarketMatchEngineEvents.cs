using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public class StockMarketMatchEngineEvents:EventArgs
    {
        public EventType eventType { get; set; }

        public object? EventObject { get; set; }
        public string Description { get; set; }

        public int Threshold { get; set; }
    }
}
