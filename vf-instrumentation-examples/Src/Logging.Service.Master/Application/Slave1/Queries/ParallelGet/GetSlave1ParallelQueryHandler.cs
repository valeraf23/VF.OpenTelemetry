using Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Slave1.Queries.ParallelGet
{
    public class GetSlave1ParallelQueryHandler : IRequestHandler<GetSlave1ParallelOrdersList, OrdersList>
    {
        private readonly ISlave1Service _services;

        public GetSlave1ParallelQueryHandler(ISlave1Service services) => _services = services;

        public async Task<OrdersList> Handle(GetSlave1ParallelOrdersList request, CancellationToken cancellationToken) =>
            await _services.Parallel(request.Buyer);
    }
}
