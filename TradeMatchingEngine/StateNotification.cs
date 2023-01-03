

namespace TradeMatchingEngine
{
    public class StateNotification
    {
        public virtual event EventHandler MarketStateChanged;

        private ChangeStateNotify changeState;

        public StateNotification()
        {
        }

        public virtual void ChangeStateMarket()
        {
            changeState = ChangeStateNotify.NormalChange;

            OnStateChanged(new MarketStateEngine() { ChangeStateNotify = changeState });
        }

        protected virtual void OnStateChanged(MarketStateEngine e)
        {
            MarketStateChanged?.Invoke(this, e);
        }
    }

    public enum ChangeStateNotify
    {
        NormalChange,
        ForcedChange
    }
}
