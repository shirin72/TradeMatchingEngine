using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class TradeMatchingEngineContext : DbContext
    {
        public TradeMatchingEngineContext(DbContextOptions<TradeMatchingEngineContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngineDb;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");
        }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //OrderEntity
            modelBuilder.Entity<TradeMatchingEngine.Order>().Property(o => o.Side).HasConversion<int>();

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("Id");

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("Side");

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("Price");

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("Amount");

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("IsFillAndKill");

            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("ExpireTime");


            modelBuilder.Entity<TradeMatchingEngine.Order>()
            .Property("OrderParentId");


            //TradeEntity

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("Id");

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("OwnerId");

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("BuyOrderId");

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("SellOrderId");

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("Amount");

            modelBuilder.Entity<TradeMatchingEngine.Trade>()
            .Property("Price");

        }

        public DbSet<TradeMatchingEngine.Order> Orders { get; set; }
        public DbSet<TradeMatchingEngine.Trade> Trades { get; set; }
    }
}
