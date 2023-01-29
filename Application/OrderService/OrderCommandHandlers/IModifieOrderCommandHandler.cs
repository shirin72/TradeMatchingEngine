using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface IModifieOrderCommandHandler
    {
        Task<long?> Handle(long orderId, int price, int amount, DateTime? expDate);

    }
}
