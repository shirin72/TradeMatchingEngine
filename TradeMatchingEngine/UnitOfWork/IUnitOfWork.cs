namespace Domain.UnitOfWork
{
    public interface IUnitOfWork:IAsyncDisposable
    {
        Task<int> SaveChange();
    }
}
