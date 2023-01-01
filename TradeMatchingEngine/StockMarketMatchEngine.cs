namespace TradeMatchingEngine
{
    public class StockMarketMatchEngine
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> SellOrderQueue;
        private readonly PriorityQueue<Order, Order> BuyOrderQueue;
        #endregion

        public StockMarketMatchEngine()
        {
            this.SellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.BuyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());
            this.Orders = new List<Order>();
        }

        #region Properties
        public List<Order> Orders { get; set; }
        public int TradeCount { get; set; }
        #endregion

        #region Method
        public void Trade(Order order)
        {
            Orders.Add(order);

            if (order.Side == Side.Buy)
            {
                Buy(order);
            }
            else if (order.Side == Side.Sell)
            {
               Sell(order);
            }
        }

        public List<Order> GetAllOrdersList()
        {
            return Orders;
        }

        public PriorityQueue<Order, Order> GetSellOrderQueue()
        {
            return SellOrderQueue;
        }

        public PriorityQueue<Order, Order> GetBuyOrderQueue()
        {
            return BuyOrderQueue;
        }

        private  void Buy(Order order)
        {
            var sellAvailble = SellOrderQueue.Count;
            if (sellAvailble > 0)
            {
                var sell = SellOrderQueue.Peek();
                if (sell.Price <= order.Price)
                {
                    TradeCount++;
                    if (order.Amount == sell.Amount)
                    {
                        SellOrderQueue.Dequeue();
                        var sellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();
                        Orders.Remove(sellOrder);
                        Orders.Remove(order);
                    }
                    else if (order.Amount < sell.Amount)
                    {
                        var newAmount = sell.Amount - order.Amount;

                        var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();
                        findSellOrder.Amount = newAmount;

                        SellOrderQueue.Dequeue();
                        SellOrderQueue.Enqueue(findSellOrder, findSellOrder);
                        Orders.Remove(order);
                    }
                    else
                    {
                        var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();

                        Orders.Remove(findSellOrder);

                        SellOrderQueue.Dequeue();

                        var findOrder = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();
                        findOrder.Amount = order.Amount - sell.Amount;

                        BuyOrderQueue.Enqueue(findOrder, findOrder);
                    }

                }
                else
                {
                    BuyOrderQueue.Enqueue(order, order);
                }
            }
            else
            {
                BuyOrderQueue.Enqueue(order, order);
            }
        }

        private void Sell(Order order)
        {
            var buyAvailble = BuyOrderQueue.Count;
            if (buyAvailble > 0)
            {
                var buy = BuyOrderQueue.Peek();
                if (buy.Price >= order.Price)
                {
                    TradeCount++;
                    if (order.Amount == buy.Amount)
                    {
                        BuyOrderQueue.Dequeue();
                        var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();
                        Orders.Remove(findBuyOrder);
                        Orders.Remove(order);
                    }
                    else if (order.Amount < buy.Amount)
                    {
                        BuyOrderQueue.Dequeue();

                        var newAmount = buy.Amount - order.Amount;

                        var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();
                        findBuyOrder.Amount = newAmount;
                        BuyOrderQueue.Enqueue(findBuyOrder, findBuyOrder);

                        Orders.Remove(order);
                    }
                    else
                    {
                        var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();

                        Orders.Remove(findBuyOrder);
                        BuyOrderQueue.Dequeue();

                        var findOrder = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();
                        findOrder.Amount = order.Amount - buy.Amount;

                        SellOrderQueue.Enqueue(findOrder, findOrder);
                    }

                }
                else
                {
                    SellOrderQueue.Enqueue(order, order);

                }
            }
            else
            {
                SellOrderQueue.Enqueue(order, order);
            }
        }

        public int GetBuyOrderCount()
        {
            return Orders.Where(x => x.Side == Side.Buy).GroupBy(x => x.Price).Count();
        }

        public int GetSellOrderCount()
        {
            return Orders.Where(x => x.Side == Side.Sell).GroupBy(x => x.Price).Count();
        }
        #endregion


    }
}
