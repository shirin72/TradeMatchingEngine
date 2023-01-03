

namespace TradeMatchingEngine
{
    public class StateNotification
    {
        #region Event
        public virtual event EventHandler MarketStateChanged;
        #endregion

        #region Private
        private ChangeStateNotify changeState;
        #endregion

        #region PublicMethod
        public virtual void ChangeStateMarket()
        {
            changeState = ChangeStateNotify.NormalChange;

            OnStateChanged(new MarketStateEngine() { ChangeStateNotify = changeState });
        }
        protected virtual void OnStateChanged(MarketStateEngine e)
        {
            MarketStateChanged?.Invoke(this, e);
        }
        #endregion
    }

    public enum ChangeStateNotify
    {
        NormalChange,
        ForcedChange
    }
}
