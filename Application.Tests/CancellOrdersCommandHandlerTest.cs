using Application.OrderService.OrderCommandHandlers;
using System;

namespace Application.Tests
{
    public class CancellOrdersCommandHandlerTest : CommandHandelerTest<CancellOrderCommandHandler, long>
    {

        protected override long MakeSomeTCommand()
        {
            return 1;
        }
    }
}
