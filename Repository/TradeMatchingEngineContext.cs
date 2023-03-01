using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{

    public class TradeMatchingEngineContext : DbContext
    {
        public TradeMatchingEngineContext(DbContextOptions<TradeMatchingEngineContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public void EnsureDatabaseIsDropped()
        {
            Database.EnsureDeleted();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //OrderEntity

            modelBuilder.Entity<Domain.Order>(b =>
            {
                b.HasKey(o => o.Id);
                b.Property(o => o.Id).ValueGeneratedNever();
                b.Property(o => o.Price);
                b.Property(o => o.Amount);
                b.Property(o => o.IsFillAndKill);
                b.Property(o => o.ExpireTime);
                b.Property(o => o.OrderState).HasConversion<int>();
                b.Property(o => o.OrderParentId);
                b.Property(o => o.Side).HasConversion<int>();
            });



            //TradeEntity

            modelBuilder.Entity<Domain.Trade>(b =>
            {
                b.HasKey(o => o.Id);
                b.Property(o => o.Id).ValueGeneratedNever();
                b.Property(o => o.Price);
                b.Property(o => o.Amount);
                b.HasOne<Domain.Order>().WithMany().HasForeignKey(t => t.BuyOrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasOne<Domain.Order>().WithMany().HasForeignKey(t => t.SellOrderId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.Property(o => o.SellOrderId);
                b.Property(o => o.BuyOrderId);
                // b.Property(o => o.OwnerId);
            });

        }

        public DbSet<Domain.Order> Orders { get; set; }
        public DbSet<Domain.Trade> Trades { get; set; }
    }
}
