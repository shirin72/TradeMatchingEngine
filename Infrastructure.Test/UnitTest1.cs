using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Infrastructure.Test
{
    public class UnitTest1 : IAsyncDisposable
    {
        private TradeMatchingEngineContext dbContext;
        private readonly ITestOutputHelper testOutput;
        public UnitTest1(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
            var optionsBuilder = new DbContextOptionsBuilder<TradeMatchingEngineContext>();
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngineDb;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");
            optionsBuilder.LogTo(data=>testOutput.WriteLine(data));
            dbContext = new TradeMatchingEngineContext(optionsBuilder.Options);
        }

        public async ValueTask DisposeAsync()
        {
            await dbContext.DisposeAsync();
        }

        [Fact]
        public void Test1()
        {
            //arrange

            //act
            var order = dbContext.Orders.Where(o => o.Id == 1).FirstOrDefault();

            //assert


        }
    }
}