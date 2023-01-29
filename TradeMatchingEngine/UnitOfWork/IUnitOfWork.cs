namespace TradeMatchingEngine.UnitOfWork
{
    public interface IUnitOfWork:IAsyncDisposable
    {
        Task<int> SaveChange();
    }
}
