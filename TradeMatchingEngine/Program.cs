using TradeMatchingEngine;

var stateNotifier = new StateNotification();
var stateController = new StateController(new StockMarketMatchEngine());
stateNotifier.MarketStateChanged += stateController.MarketState_HasChanged;

var startTimeSpan = TimeSpan.Zero;
var periodTimeSpan = TimeSpan.FromSeconds(10);

var timer = new System.Threading.Timer((e) =>
{
    stateNotifier.ChangeStateMarket();
}, null, startTimeSpan, periodTimeSpan);

Console.ReadKey();

