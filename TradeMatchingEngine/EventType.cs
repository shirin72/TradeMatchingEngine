using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public enum EventType
    {
        TradeExecuted,
        MarketOpened,
        MarketClosed,
        MarketPreOpened,
        OrderEnqued,
        OrderExpired
    }
}
