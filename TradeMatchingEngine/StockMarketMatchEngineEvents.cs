namespace TradeMatchingEngine
{
    public class StockMarketMatchEngineEvents:EventArgs
    {
        public EventType eventType { get; set; }
        public object? EventObject { get; set; }
        public string Description { get; set; }
    }
}
