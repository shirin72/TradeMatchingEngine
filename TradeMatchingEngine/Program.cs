// See https://aka.ms/new-console-template for more information
using TradeMatchingEngine;

var buyOrder1 = new Order()
{
    Amount = 50,
    Id = 1,
    Price = 100,
    Side = Side.Buy,
};

var buyOrder2 = new Order()
{
    Amount = 50,
    Id = 2,
    Price = 100,
    Side = Side.Buy,
};


var stockMarket = new StockMarketMatchEngine();
stockMarket.Trade(buyOrder1);
stockMarket.Trade(buyOrder2);
stockMarket.GetAllOrdersList();
stockMarket.GetBuyOrderQueue();
Console.WriteLine("Hello, World!");
