namespace Domain.Orders.Repositories.Command
{
    public interface ICommandRepository<T,TInterface> where T:TInterface
    {
        //Task<T> Find(long id);
        Task<TInterface> Find(long id);
        Task Add(T order);
        Task Add(TInterface order);
        Task Delete(long id);
    }
}
