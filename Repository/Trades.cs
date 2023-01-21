using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository
{
    [Table("Order")]
    public class Trades: Base
    {
        [Required]
        public int OwnerId { get; set; }
        [Required]
        public int BuyOrderId { get; set; }
        [Required]
        public int SellOrderId { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Price { get; set; }
    }
}