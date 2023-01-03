namespace TradeMatchingEngine
{
    public class StockMarketMatchEngine
    {
        #region PrivateField
        private readonly PriorityQueue<Order, Order> SellOrderQueue;
        private readonly PriorityQueue<Order, Order> BuyOrderQueue;
        private MarketStateEnum marketState;

        private readonly Queue<Order> preOrderQueue;
        #endregion

        public StockMarketMatchEngine()
        {
            this.SellOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMin());
            this.BuyOrderQueue = new PriorityQueue<Order, Order>(new ModifiedOrderPriorityMax());
            this.Orders = new List<Order>();
            this.marketState = MarketStateEnum.Close;
            preOrderQueue = new Queue<Order>();
        }

        #region Properties
        public List<Order> Orders { get; set; }
        public int TradeCount { get; set; }
        #endregion

        #region Method
        public void Trade(Order order)
        {
            switch (marketState)
            {
                case MarketStateEnum.PreOpen:
                    preOrderQueue.Enqueue(order);
                    Orders.Add(order);
                    break;
                case MarketStateEnum.Open:
                    if (preOrderQueue.Count > 0)
                    {
                        int cnt = preOrderQueue.Count;
                        for (int i = 0; i <cnt; i++)
                        {
                            var preOrderdOrder = (preOrderQueue.Peek());

                            if (preOrderdOrder.Side == Side.Buy)
                            {
                                Buy(preOrderdOrder);
                            }
                            else if (preOrderdOrder.Side == Side.Sell)
                            {
                                Sell(preOrderdOrder);
                            }

                            preOrderQueue.Dequeue();
                        }
                    }
                    else
                    {
                        if (order != null)
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
                    }
                    break;
                case MarketStateEnum.Close:
                    throw new Exception("This Stock Has been Closed");
                default:
                    break;
            }
        }

        public virtual MarketStateEnum GetCurrentMarketState()
        {
            return marketState;
        }
        public List<Order> GetAllOrdersList()
        {
            return Orders;
        }

        public PriorityQueue<Order, Order> GetSellOrderQueue()
        {
            return SellOrderQueue;
        }
        public Queue<Order> GetPreOrderQueue()
        {
            return preOrderQueue;
        }
        public PriorityQueue<Order, Order> GetBuyOrderQueue()
        {
            return BuyOrderQueue;
        }

        private void Buy(Order order)
        {
            var sellAvailble = SellOrderQueue.Count;
            if (sellAvailble > 0)
            {
                var sell = SellOrderQueue.Peek();

                if (order.Price >= sell.Price)
                {
                    if (order.Amount == sell.Amount)
                    {
                        TradeCount++;
                        SellOrderQueue.Dequeue();
                        Orders.Remove(order);
                        Orders.Remove(sell);
                    }
                    if (order.Amount > sell.Amount)
                    {
                        TradeCount++;
                        SellOrderQueue.Dequeue();

                        Orders.Remove(sell);

                        order.Amount = order.Amount - sell.Amount;
                        Orders.Where(o => o.Id == order.Id).First().Amount = order.Amount;

                        while (order.Amount > 0)
                        {
                            if (SellOrderQueue.Count > 0)
                            {

                                var nextSell = SellOrderQueue.Peek();
                                if (nextSell.Price > order.Price)
                                {
                                    if (BuyOrderQueue.Count > 0)
                                    {
                                        if (BuyOrderQueue.Peek().Id == order.Id)
                                        {
                                            BuyOrderQueue.Dequeue();
                                            BuyOrderQueue.Enqueue(order, order);
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
                                    break;
                                }
                                if (order.Amount == nextSell.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(nextSell);
                                    Orders.Remove(order);
                                    SellOrderQueue.Dequeue();
                                    break;
                                }

                                else if (order.Amount < nextSell.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(order);
                                    nextSell.Amount = nextSell.Amount - order.Amount;
                                    var sellUpdate = Orders.Where(o => o.Id == nextSell.Id).First();
                                    sellUpdate.Amount = nextSell.Amount - order.Amount;
                                    order.Amount = 0;
                                    SellOrderQueue.Dequeue();
                                    SellOrderQueue.Enqueue(nextSell, nextSell);
                                    if (BuyOrderQueue.Count > 0)
                                    {
                                        if (BuyOrderQueue.Peek().Id == order.Id)
                                        {
                                            BuyOrderQueue.Dequeue();
                                        }
                                    }

                                }

                                else if (order.Amount > nextSell.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(nextSell);
                                    SellOrderQueue.Dequeue();
                                    order.Amount = order.Amount - nextSell.Amount;
                                    if (BuyOrderQueue.Count > 0)
                                    {
                                        if (BuyOrderQueue.Peek().Id == order.Id)
                                        {
                                            BuyOrderQueue.Dequeue();
                                            BuyOrderQueue.Enqueue(order, order);
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
                            }
                            else
                            {
                                if (BuyOrderQueue.Count > 0)
                                {
                                    if (BuyOrderQueue.Peek().Id == order.Id)
                                    {
                                        BuyOrderQueue.Dequeue();
                                        BuyOrderQueue.Enqueue(order, order);
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
                                break;
                            }
                        }
                    }

                    else if (order.Amount < sell.Amount)
                    {
                        TradeCount++;
                        Orders.Remove(order);
                        sell.Amount = sell.Amount - order.Amount;
                        var sellUpdate = Orders.Where(o => o.Id == sell.Id).First();
                        sellUpdate.Amount = sell.Amount - order.Amount;

                        SellOrderQueue.Dequeue();
                        SellOrderQueue.Enqueue(sell, sell);

                    }
                }
                else
                {

                    BuyOrderQueue.Enqueue(order, order);

                }


                #region Commented Old Buy procedure
                //    if (sell.Price <= order.Price)
                //    {
                //        TradeCount++;
                //        if (order.Amount == sell.Amount)
                //        {
                //            SellOrderQueue.Dequeue();
                //            var sellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();
                //            Orders.Remove(sellOrder);
                //            Orders.Remove(order);
                //        }
                //        else if (order.Amount < sell.Amount)
                //        {
                //            var newAmount = sell.Amount - order.Amount;

                //            var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();
                //            findSellOrder.Amount = newAmount;

                //            SellOrderQueue.Dequeue();
                //            SellOrderQueue.Enqueue(findSellOrder, findSellOrder);
                //            Orders.Remove(order);
                //        }
                //        else
                //        {
                //            var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();

                //            Orders.Remove(findSellOrder);

                //            SellOrderQueue.Dequeue();

                //            var findOrder = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();
                //            findOrder.Amount = order.Amount - sell.Amount;

                //            BuyOrderQueue.Enqueue(findOrder, findOrder);
                //        }

                //    }
                //    else
                //    {
                //        BuyOrderQueue.Enqueue(order, order);
                //    }
                //}
                //else
                //{
                //    BuyOrderQueue.Enqueue(order, order);
                //}
                #endregion

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
                if (order.Price <= buy.Price)
                {

                    if (order.Amount == buy.Amount)
                    {


                        TradeCount++;
                        Orders.Remove(buy);
                        Orders.Remove(order);
                        BuyOrderQueue.Dequeue();

                    }
                    if (order.Amount > buy.Amount)
                    {
                        TradeCount++;
                        BuyOrderQueue.Dequeue();
                        Orders.Remove(buy);
                        order.Amount = order.Amount - buy.Amount;
                        Orders.Where(o => o.Id == order.Id).First().Amount = order.Amount;

                        while (order.Amount > 0)
                        {
                            if (BuyOrderQueue.Count > 0)
                            {

                                var nextBuy = BuyOrderQueue.Peek();
                                if (nextBuy.Price < order.Price)
                                {
                                    if (SellOrderQueue.Count > 0)
                                    {
                                        if (SellOrderQueue.Peek().Id == order.Id)
                                        {
                                            SellOrderQueue.Dequeue();
                                            SellOrderQueue.Enqueue(order, order);
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
                                    break;
                                }
                                if (order.Amount == nextBuy.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(nextBuy);
                                    Orders.Remove(order);
                                    BuyOrderQueue.Dequeue();
                                    break;
                                }

                                else if (order.Amount < nextBuy.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(order);
                                    nextBuy.Amount = nextBuy.Amount - order.Amount;
                                    var buyUpdate = Orders.Where(o => o.Id == nextBuy.Id).First();
                                    buyUpdate.Amount = nextBuy.Amount;
                                    order.Amount = 0;
                                    BuyOrderQueue.Dequeue();
                                    BuyOrderQueue.Enqueue(nextBuy, nextBuy);


                                }

                                else if (order.Amount > nextBuy.Amount)
                                {
                                    TradeCount++;
                                    Orders.Remove(nextBuy);
                                    BuyOrderQueue.Dequeue();
                                    order.Amount = order.Amount - nextBuy.Amount;

                                    if (SellOrderQueue.Count > 0)
                                    {
                                        if (SellOrderQueue.Peek().Id == order.Id)
                                        {
                                            SellOrderQueue.Dequeue();
                                            SellOrderQueue.Enqueue(order, order);
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
                            else
                            {
                                if (SellOrderQueue.Count > 0)
                                {
                                    if (SellOrderQueue.Peek().Id == order.Id)
                                    {
                                        SellOrderQueue.Dequeue();
                                        SellOrderQueue.Enqueue(order, order);
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
                                break;
                            }
                        }
                    }
                    else if (order.Amount < buy.Amount)
                    {
                        TradeCount++;
                        Orders.Remove(order);
                        buy.Amount = buy.Amount - order.Amount;
                        var sellUpdate = Orders.Where(o => o.Id == buy.Id).First();
                        sellUpdate.Amount = buy.Amount - order.Amount;


                        BuyOrderQueue.Dequeue();
                        BuyOrderQueue.Enqueue(buy, buy);
                        if (SellOrderQueue.Count > 0)
                        {
                            if (SellOrderQueue.Peek().Id == order.Id)
                            {
                                SellOrderQueue.Dequeue();
                            }
                        }
                    }


                }
                else
                {
                    SellOrderQueue.Enqueue(order, order);
                }
                #region Commented Old Sell Procedure
                //    if (buy.Price >= order.Price)
                //    {
                //        TradeCount++;
                //        if (order.Amount == buy.Amount)
                //        {
                //            BuyOrderQueue.Dequeue();
                //            var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();
                //            Orders.Remove(findBuyOrder);
                //            Orders.Remove(order);
                //        }
                //        else if (order.Amount < buy.Amount)
                //        {
                //            BuyOrderQueue.Dequeue();

                //            var newAmount = buy.Amount - order.Amount;

                //            var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();
                //            findBuyOrder.Amount = newAmount;
                //            BuyOrderQueue.Enqueue(findBuyOrder, findBuyOrder);

                //            Orders.Remove(order);
                //        }
                //        else
                //        {
                //            var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();

                //            Orders.Remove(findBuyOrder);
                //            BuyOrderQueue.Dequeue();

                //            var findOrder = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();
                //            findOrder.Amount = order.Amount - buy.Amount;

                //            SellOrderQueue.Enqueue(findOrder, findOrder);
                //        }

                //    }
                //    else
                //    {
                //        SellOrderQueue.Enqueue(order, order);

                //    }
                //}
                //else
                //{
                //    SellOrderQueue.Enqueue(order, order);
                //}
                #endregion
            }
            else
            {
                SellOrderQueue.Enqueue(order, order);
            }
        }


        public int GetBuyOrderCount()
        {
            // return Orders.Where(x => x.Side == Side.Buy).GroupBy(x => x.Price).Count();
            return BuyOrderQueue.Count;
        }

        public int GetSellOrderCount()
        {
            // return Orders.Where(x => x.Side == Side.Sell).GroupBy(x => x.Price).Count();
            return SellOrderQueue.Count;
        }

        public virtual void SetState(MarketStateEnum marketStateEnum)
        {
            this.marketState = marketStateEnum;

            if (marketStateEnum == MarketStateEnum.Close)
            {
                BuyOrderQueue.Clear();
                SellOrderQueue.Clear();
            }

            if (marketState == MarketStateEnum.Open)
            {
                if (preOrderQueue.Count > 0)
                {
                    Trade(null);
                }
            }
        }

        #endregion


    }
}
