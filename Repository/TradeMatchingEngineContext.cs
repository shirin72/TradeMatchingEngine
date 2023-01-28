using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{

    public class TradeMatchingEngineContext : DbContext
    {
        public TradeMatchingEngineContext(DbContextOptions<TradeMatchingEngineContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //OrderEntity

            modelBuilder.Entity<TradeMatchingEngine.Order>(b =>
            {
                b.HasKey(o => o.Id);
                b.Property(o => o.Id).ValueGeneratedNever();
                b.Property(o => o.Price);
                b.Property(o => o.Amount);
                b.Property(o => o.IsFillAndKill);
                b.Property(o => o.ExpireTime);
                b.Property(o => o.OrderParentId);
                b.Property(o => o.Side).HasConversion<int>();
                b.Property(o => o.OrderState).HasConversion<int>();
            });



            //TradeEntity

            modelBuilder.Entity<TradeMatchingEngine.Trade>(b =>
            {
                b.HasKey(o => o.Id);
                b.Property(o => o.Id).ValueGeneratedNever();
                b.Property(o => o.Price);
                b.Property(o => o.Amount);
                b.HasOne<TradeMatchingEngine.Order>().WithMany().HasForeignKey(t => t.BuyOrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasOne<TradeMatchingEngine.Order>().WithMany().HasForeignKey(t => t.SellOrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.Property(o => o.SellOrderId)   ;
                b.Property(o => o.BuyOrderId);
                // b.Property(o => o.OwnerId);
            });

        }

        public DbSet<TradeMatchingEngine.Order> Orders { get; set; }
        public DbSet<TradeMatchingEngine.Trade> Trades { get; set; }
    }
}
