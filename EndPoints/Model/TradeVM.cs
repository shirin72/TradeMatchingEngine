using Domain;

namespace EndPoints.Model
{
    public class TradeVM
    {
        public TradeVM()
        {
            Link = new List<LinkVM>();
        }
        public int Amount { get; set; }
        public long BuyOrderId { get; set; }
        public int Price { get; set; }
        public long SellOrderId { get; set; }
        public long Id { get; set; }
        public List<LinkVM> Link { get; set; }
    }
}
