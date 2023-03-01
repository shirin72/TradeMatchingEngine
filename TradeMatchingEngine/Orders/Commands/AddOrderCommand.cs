namespace Domain.Orders.Commands
{
    public class AddOrderCommand : ICommands
    {
        public int Price { get; set; }
        public int Amount { get; set; }
        public Side Side { get; set; }
        public DateTime? ExpDate { get; set; }
        public bool IsFillAndKill { get; set; }
        public long? orderParentId {get;set;}
    }
}
