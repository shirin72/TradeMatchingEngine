namespace EndPoints.Model
{
    public class RegisteredOrderVM
    {
        public RegisteredOrderVM()
        {
            Links = new List<LinkVM>();
        }

        public long OrderId { get; set; }

        public IEnumerable<LinkVM> Links { get; set; }
    }
}
