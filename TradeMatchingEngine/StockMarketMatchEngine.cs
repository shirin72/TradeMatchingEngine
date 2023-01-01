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
                       

                        var orderSellOrdersList = Orders.Where(o => o.Price <= sell.Price && o.Side == Side.Sell).OrderBy(o => o.Price).ToList();
                        bool hasBuyEnqued = false;

                        for (int i = 0; i < orderSellOrdersList.Count; i++)
                        {
                            if (orderSellOrdersList[i].Amount == order.Amount)
                            {
                                TradeCount++;
                                Orders.Remove(orderSellOrdersList[i]);
                                Orders.Remove(order);
                                var top = SellOrderQueue.Peek();
                                if (top.Price == order.Price)
                                {
                                    SellOrderQueue.Dequeue();
                                }
                                if(BuyOrderQueue.Count>0)
                                {
                                    if (BuyOrderQueue.Peek().Price == order.Price)
                                    {
                                        BuyOrderQueue.Dequeue();
                                    }
                                    
                                }
                                break;
                            }
                            else if (orderSellOrdersList[i].Amount > order.Amount)
                            {
                                TradeCount++;
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
                                TradeCount++;
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
                                        if (order.Amount > 0)
                                        {
                                            BuyOrderQueue.Enqueue(price, price);
                                            hasBuyEnqued = true;
                                        }

                                    }
                                }
                            }
                        }

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

                        
                        bool hasSellEnqued = false;
                        for (int i = 0; i < orderBuyOrdersList.Count; i++)
                        {
                            if (orderBuyOrdersList[i].Amount == order.Amount)
                            {
                                TradeCount++;
                                Orders.Remove(orderBuyOrdersList[i]);
                                Orders.Remove(order);
                                var top = BuyOrderQueue.Peek();
                                if (top.Price == order.Price)
                                {
                                    BuyOrderQueue.Dequeue();
                                }

                                if (SellOrderQueue.Count > 0)
                                {
                                    if (SellOrderQueue.Peek().Price == order.Price)
                                    {
                                        SellOrderQueue.Dequeue();
                                    }

                                }



                                break;
                            }
                            else if (orderBuyOrdersList[i].Amount > order.Amount)
                            {
                                TradeCount++;
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
                                TradeCount++;
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
