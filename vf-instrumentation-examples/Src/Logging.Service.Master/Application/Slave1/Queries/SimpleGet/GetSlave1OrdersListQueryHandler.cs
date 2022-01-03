using Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Slave1.Queries.SimpleGet
{
    public class GetSlave1OrdersListQueryHandler : IRequestHandler<GetSlave1OrdersList, OrdersList>
    {
        private readonly ISlave1Service _services;

        public GetSlave1OrdersListQueryHandler(ISlave1Service services) => _services = services;

        public async Task<OrdersList> Handle(GetSlave1OrdersList request, CancellationToken cancellationToken) =>
            await _services.Get();
    }
}
