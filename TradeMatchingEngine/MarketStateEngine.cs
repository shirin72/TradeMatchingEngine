using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public class MarketStateEngine:EventArgs
    {
        public ChangeStateNotify ChangeStateNotify { get; set; }
    }
}
