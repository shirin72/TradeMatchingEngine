using Infrastructure;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine;

namespace Infrastructure
{
    public class TradeMatchingEngineContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngine;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TradeOrder>().HasKey(sc => new { sc.TradeId, sc.OrderId });
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<ITrade> Trades { get; set; }
    }
}
