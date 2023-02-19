using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class TestCommandHandlerTests : CommandHandelerTest<TestCommandHandler, TestCommand>
    {
        public TestCommandHandlerTests()
        {

        }

        protected override TestCommand MakeSomeTCommand()
        {
            return new TestCommand();
        }
    }
}