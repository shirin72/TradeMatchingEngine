using Domain;

namespace Application.Tests
{
    public class TestTrade : ITrade
    {
        public int Amount { get; set; }

        public long BuyOrderId { get; set; }

        public int Price { get; set; }

        public long SellOrderId { get; set; }

        public long Id { get; set; }
    }
}