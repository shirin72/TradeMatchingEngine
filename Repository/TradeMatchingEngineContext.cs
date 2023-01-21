using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TradeMatchingEngineContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=TradeMatchingEngineDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<Trades> Trades { get; set; }
    }
}
