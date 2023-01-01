// See https://aka.ms/new-console-template for more information
using TradeMatchingEngine;

var buyOrder = new Order()
{
    Amount = 80,
    Id = 4,
    Price = 120,
    Side = Side.Buy,
};

var buyOrder1 = new Order()
{
    Amount = 50,
    Id = 1,
    Price = 120,
    Side = Side.Buy,
};

var buyOrder2 = new Order()
{
    Amount = 50,
    Id = 2,
    Price = 100,
    Side = Side.Buy,
};

var sellOrder3 = new Order()
{
    Amount = 190,
    Id = 5,
    Price = 90,
    Side = Side.Sell,
};

//var sellOrder1 = new Order()
//{
//    Amount = 40,
//    Id = 6,
//    Price = 90,
//    Side = Side.Sell,
//};



var stockMarket = new StockMarketMatchEngine();
stockMarket.Trade(buyOrder1);
stockMarket.Trade(buyOrder2);
stockMarket.Trade(buyOrder);

stockMarket.Trade(sellOrder3);
//stockMarket.Trade(sellOrder1);

//stockMarket.GetAllOrdersList();
stockMarket.GetBuyOrderQueue();
Console.WriteLine("Hello, World!");
