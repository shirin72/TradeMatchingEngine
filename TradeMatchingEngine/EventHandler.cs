namespace TradeMatchingEngine
{
    public class EventHandler
    {
        public  void bl_ProcessCompleted(object sender, EventArgs e)
        {
            var result = e as StockMarketMatchEngineEvents;
            Console.WriteLine("Process Completed! {0}", result?.EventType.ToString());
        }
    }
}
