using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure
{
    public class StockMarketContextFactory : IDesignTimeDbContextFactory<TradeMatchingEngineContext>
    {
        public TradeMatchingEngineContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TradeMatchingEngineContext>();
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngineDesignDb;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");

            return new TradeMatchingEngineContext(optionsBuilder.Options);
        }
    }
}
