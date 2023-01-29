namespace TradeMatchingEngine.Orders.Repositories.Command
{
    public interface ICommandRepository<T>
    {
        Task<T> Find(long id);
        Task Add(T order);
        Task Delete(long id);
    }
}
