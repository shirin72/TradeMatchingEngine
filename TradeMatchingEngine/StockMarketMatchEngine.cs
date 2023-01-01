namespace TradeMatchingEngine
{
    public class StockMarketMatchEngine
    {
        private readonly PriorityQueue<PriceInQueue, PriceInQueue> SellOrderQueue;
        private readonly PriorityQueue<PriceInQueue, PriceInQueue> BuyOrderQueue;

        public StockMarketMatchEngine()
        {
            this.SellOrderQueue = new PriorityQueue<PriceInQueue, PriceInQueue>(new ModifiedOrderPriorityMin());
            this.BuyOrderQueue = new PriorityQueue<PriceInQueue, PriceInQueue>(new ModifiedOrderPriorityMax());
            this.Orders = new List<Order>();
        }

        public List<Order> Orders { get; set; }
        public int TradeCount { get; set; }

        public void Trade(Order order)
        {
            Orders.Add(order);


            var price = new PriceInQueue();
            price.Price = order.Price;

            if (order.Side == Side.Buy)
            {
                var sellAvailble = SellOrderQueue.Count;
                if (sellAvailble > 0)
                {
                    var sell = SellOrderQueue.Peek();
                    if (sell.Price <= order.Price)
                    {
                        TradeCount++;

                        var orderSellOrdersList = Orders.Where(o => o.Price >= sell.Price && o.Side == Side.Sell).OrderBy(o => o.Price).ToList();
                        bool hasBuyEnqued = false;

                        for (int i = 0; i < orderSellOrdersList.Count; i++)
                        {
                            if (orderSellOrdersList[i].Amount == order.Amount)
                            {
                                Orders.Remove(orderSellOrdersList[i]);
                                Orders.Remove(order);
                                var top = SellOrderQueue.Peek();
                                if (top.Price == order.Price)
                                {
                                    SellOrderQueue.Dequeue();
                                }

                                break;
                            }
                            else if (orderSellOrdersList[i].Amount > order.Amount)
                            {
                                orderSellOrdersList[i].Amount = orderSellOrdersList[i].Amount - order.Amount;
                                Orders.Remove(order);

                                if (BuyOrderQueue.Count > 0)
                                {
                                    var top = BuyOrderQueue.Peek();
                                    if (top.Price == order.Price)
                                    {
                                        BuyOrderQueue.Dequeue();
                                    }
                                }
                                break;
                            }
                            else
                            {
                                order.Amount = order.Amount - orderSellOrdersList[i].Amount;
                                Orders.Remove(orderSellOrdersList[i]);

                                var top = SellOrderQueue.Peek();
                                if (top.Price <= order.Price)
                                {
                                    if (orderSellOrdersList.Where(o => o.Price == top.Price).ToList().Count == 1)
                                    {
                                        SellOrderQueue.Dequeue();
                                    }
                                    if (!hasBuyEnqued)
                                    {
                                        BuyOrderQueue.Enqueue(price, price);
                                        hasBuyEnqued = true;
                                    }
                                }
                            }
                        }
                        //if (order.Amount == sell.Amount)
                        //{
                        //    SellOrderQueue.Dequeue();
                        //    var sellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();
                        //    Orders.Remove(sellOrder);
                        //    Orders.Remove(order);
                        //}
                        //else if (order.Amount < sell.Amount)
                        //{
                        //    var newAmount = sell.Amount - order.Amount;

                        //    var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();

                        //    findSellOrder.Amount = newAmount;

                        //    SellOrderQueue.Dequeue();
                        //    SellOrderQueue.Enqueue(findSellOrder, findSellOrder);
                        //    Orders.Remove(order);
                        //}
                        //else
                        //{
                        //    var findSellOrder = Orders.Where(x => x.Id == sell.Id && x.Side == sell.Side).FirstOrDefault();

                        //    Orders.Remove(findSellOrder);

                        //    SellOrderQueue.Dequeue();

                        //    var res1 = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();

                        //    res1.Amount = order.Amount - sell.Amount;

                        //    BuyOrderQueue.Enqueue(res1, res1);
                        //}

                    }
                    else
                    {
                        if (Orders.Where(o => o.Price == order.Price && o.Side==order.Side).ToList().Count==1)
                        {
                            BuyOrderQueue.Enqueue(price, price);
                        }
                    }

                }
                else
                {
                    if (Orders.Where(o => o.Price == order.Price).Count() < 2)
                    {
                        BuyOrderQueue.Enqueue(price, price);
                    }

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
                        var orderBuyOrdersList = Orders.Where(o => o.Price <= buy.Price && o.Side == Side.Buy).OrderByDescending(o => o.Price).ToList();

                        TradeCount++;
                        bool hasSellEnqued = false;
                        for (int i = 0; i < orderBuyOrdersList.Count; i++)
                        {
                            if (orderBuyOrdersList[i].Amount == order.Amount)
                            {
                                Orders.Remove(orderBuyOrdersList[i]);
                                Orders.Remove(order);
                                var top = BuyOrderQueue.Peek();
                                if (top.Price == order.Price)
                                {
                                    BuyOrderQueue.Dequeue();
                                }

                                break;
                            }
                            else if (orderBuyOrdersList[i].Amount > order.Amount)
                            {
                                orderBuyOrdersList[i].Amount = orderBuyOrdersList[i].Amount - order.Amount;
                                Orders.Remove(order);

                                if (SellOrderQueue.Count > 0)
                                {
                                    var top = SellOrderQueue.Peek();
                                    if (top.Price == order.Price)
                                    {
                                        SellOrderQueue.Dequeue();
                                    }
                                }
                              
                                break;
                            }
                            else
                            {
                                order.Amount = order.Amount - orderBuyOrdersList[i].Amount;
                                Orders.Remove(orderBuyOrdersList[i]);

                                var top = BuyOrderQueue.Peek();
                                if (top.Price >= order.Price)
                                {
                                    if (orderBuyOrdersList.Where(o => o.Price == top.Price).ToList().Count == 1)
                                    {
                                        BuyOrderQueue.Dequeue();
                                    }
                                    if (!hasSellEnqued)
                                    {
                                        SellOrderQueue.Enqueue(price, price);
                                        hasSellEnqued = true;
                                    }
                                }

                            }
                        }
                        //if (order.Amount == buy.Amount)
                        //{
                        //    BuyOrderQueue.Dequeue();
                        //    var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();
                        //    Orders.Remove(findBuyOrder);
                        //    Orders.Remove(order);
                        //}
                        //else if (order.Amount < buy.Amount)
                        //{
                        //    BuyOrderQueue.Dequeue();

                        //    var newAmount = buy.Amount - order.Amount;

                        //    var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();

                        //    findBuyOrder.Amount = newAmount;
                        //    BuyOrderQueue.Enqueue(findBuyOrder, findBuyOrder);

                        //    Orders.Remove(order);
                        //}
                        //else
                        //{
                        //    var findBuyOrder = Orders.Where(x => x.Id == buy.Id && x.Side == buy.Side).FirstOrDefault();

                        //    Orders.Remove(findBuyOrder);
                        //    BuyOrderQueue.Dequeue();

                        //    var res1 = Orders.Where(x => x.Id == order.Id && x.Side == order.Side).FirstOrDefault();

                        //    res1.Amount = order.Amount - buy.Amount;

                        //    SellOrderQueue.Enqueue(res1, res1);
                        //}

                    }
                    else
                    {
                        SellOrderQueue.Enqueue(price, price);

                    }
                }
                else
                {
                    if (Orders.Where(o => o.Price == order.Price && o.Side == order.Side).ToList().Count==1)
                    {
                        SellOrderQueue.Enqueue(price, price);
                    }

                }
            }

            Orders = Orders.OrderBy(x => x.Id).ToList();
        }


        public List<Order> GetAllOrdersList()
        {
            return Orders;
        }


        public PriorityQueue<PriceInQueue, PriceInQueue> GetSellOrderQueue()
        {
            return SellOrderQueue;
        }

        public PriorityQueue<PriceInQueue, PriceInQueue> GetBuyOrderQueue()
        {
            return BuyOrderQueue;
        }


    }
}
