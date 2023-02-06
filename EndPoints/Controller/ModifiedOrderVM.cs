namespace EndPoints.Controller
{
    public class ModifiedOrderVM
    {
        public long OrderId { get; set; }

        public int Price { get; set; }

        public int Amount { get; set; }

        public DateTime? ExpDate { get; set; }
    }
}
