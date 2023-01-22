namespace TradeMatchingEngine.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChange();
    }
}
