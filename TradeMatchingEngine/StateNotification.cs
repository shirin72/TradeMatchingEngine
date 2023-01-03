

namespace TradeMatchingEngine
{
    public class StateNotification
    {
        public event EventHandler MarketStateChanged;

        private MarketStateEnum MarketState;

        private int StateCounter;

        public StateNotification()
        {
            this.StateCounter = 1;
        }

        public void ChangeStateMarket()
        {
            if (StateCounter == 1)
            {
                MarketState = MarketStateEnum.PreOpen;
                StateCounter++;
            }
            else if (StateCounter == 2)
            {
                MarketState = MarketStateEnum.Open;
                StateCounter++;
            }
            else
            {
                MarketState = MarketStateEnum.Close;
                StateCounter = 1;
            }

            Console.WriteLine(MarketState);

            OnStateChanged(EventArgs.Empty);
        }

        protected virtual void OnStateChanged(EventArgs e) 
        {
            MarketStateChanged?.Invoke(this,e);
        }
    }
}
