using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    public class TradeInfo:EventArgs
    {
        public int TradeId { get; set; }
        public int OwnerId { get; set; }
        public int BuyOrderId { get; set; }
        public int SellOrderId { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
    }
}
