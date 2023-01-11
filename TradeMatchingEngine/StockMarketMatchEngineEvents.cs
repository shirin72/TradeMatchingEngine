namespace TradeMatchingEngine
{
    public class StockMarketMatchEngineEvents : EventArgs
    {
        public EventType EventType { get; set; }
        public object? EventObject { get; set; }
        public string Description { get; set; }
    }
}
