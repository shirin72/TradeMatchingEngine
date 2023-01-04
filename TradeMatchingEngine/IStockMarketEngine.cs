using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeMatchingEngine
{
    internal interface IStockMarketEngine
    {
        void Open();
        void Close();
    }
}
