using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Slave1;
using Application.Slave2;
using Domain.Extensions;
using Domain.ValueObjects;
using MediatR;

namespace Application.Master.Orders.Queries.ParallelToServices
{
    public class GetOrdersListFromParallel2ServicesQueryHandler : IRequestHandler<GetOrdersListFromParallel2Services, OrdersList>
    {
        private readonly ISlave1Service _services1;
        private readonly ISlave2Service _services2;

        public GetOrdersListFromParallel2ServicesQueryHandler(ISlave1Service services1, ISlave2Service services2)
        {
            _services1 = services1;
            _services2 = services2;

        }

        public async Task<OrdersList> Handle(GetOrdersListFromParallel2Services request,
            CancellationToken cancellationToken)
        {
            var task1 = _services1.Get();
            var task2 = _services2.Get();
            Task.WaitAll(task1, task2);
            var res1 = await task1;
            var res2 = await task2;
            return res2.Orders.Concat(res1.Orders).ToOrderList();
        }
    }
}
