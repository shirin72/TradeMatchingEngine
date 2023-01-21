using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repository
{
    [Table("Order")]
    public class Orders:Base
    {
        [Required]
        public string Side { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Amount { get; set; }
        public bool? IsFillAndKill { get; set; }
        [Required]
        public DateTime ExpireTime { get; set; }
        [Required]
        public bool IsExpired { get; set; }
        [Required]
        public bool HasCompleted { get; set; }
    }
}