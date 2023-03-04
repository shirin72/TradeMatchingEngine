using Domain;

namespace EndPoints.Model
{
    public class ProcessedOrderDto
    {
        public ProcessedOrderDto()
        {
            Links = new List<LinkDto>();
        }

        public long OrderId { get; set; }

        public IEnumerable<ITrade>? Trades { get; set; }

        public IEnumerable<long> CancelledOrders { get; set; }

        public List<LinkDto> Links { get; set; }

    }
}
