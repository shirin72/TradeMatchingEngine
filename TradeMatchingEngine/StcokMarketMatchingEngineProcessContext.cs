using TradeMatchingEngine;

namespace TradeMatchingEngine
{
    public class StockMarketMatchingEngineProcessContext : IStockMarketMatchingEngineProcessContext
    {
        #region Private
        private Order createdOrder;
        private List<Order> modifiedOrders;
        private List<Trade> createdTrades;
        #endregion

        public StockMarketMatchingEngineProcessContext()
        {
            this.modifiedOrders = new List<Order>();
            this.createdTrades = new List<Trade>();
        }

        #region Public
        public Order Order => createdOrder;
        public IEnumerable<Order> ModifiedOrders => modifiedOrders;
        public IEnumerable<Trade> CreatedTrades => createdTrades;

        #endregion

        #region Internal
        internal void OrderCreated(Order order)
        {
            createdOrder = order;
        }

        internal void OrderModified(Order order)
        {
            modifiedOrders.Add(order);
        }

        internal void TradeCreated(Trade trade)
        {
            createdTrades.Add(trade);
        }
        #endregion
    }
}
