namespace EndPoints.Model
{
    public class RegisteredOrderVM
    {
        public IEnumerable<LinkVM> Links { get; set; }

        public long OrderId { get; set; }
    }
}
