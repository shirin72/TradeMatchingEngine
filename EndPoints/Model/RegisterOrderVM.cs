using Domain;

namespace EndPoints.Model
{
    public class RegisterOrderVM 
    {
        public long Id { get; set; }

        public Side Side { get; set; }

        public int Price { get; set; }

        public int Amount { get; set; }

        public bool? IsFillAndKill { get; set; } = false;

        public DateTime ExpireTime { get; set; }
    }
}
