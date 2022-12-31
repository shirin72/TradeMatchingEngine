namespace TradeMatchingEngine
{
    public class StockMarketMatchEngine
    {
        private readonly PriorityQueue<Order, Order> SellOrderQueue;
        private readonly PriorityQueue<Order, Order> BuyOrderQueue;

        public StockMarketMatchEngine()
        {
            this.SellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.BuyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());
            this.Orders = new List<Order>();
        }

        public List<Order> Orders { get; set; }
        public int TradeCount { get; set; }

        public void Trade(Order order)
        {
            Orders.Add(order);

            if (order.Side == Side.Buy)
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

                            var res1 = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();

                            res1.Amount = order.Amount - sell.Amount;

                            BuyOrderQueue.Enqueue(res1, res1);
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
            else if (order.Side == Side.Sell)
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

                            var res1 = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();

                            res1.Amount = order.Amount - buy.Amount;

                            SellOrderQueue.Enqueue(res1, res1);
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


    }
}
