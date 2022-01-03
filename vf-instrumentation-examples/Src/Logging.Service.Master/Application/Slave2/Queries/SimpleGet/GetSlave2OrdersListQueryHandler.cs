using Domain.ValueObjects;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Slave2.Queries.SimpleGet
{
    public class GetSlave2OrdersListQueryHandler : IRequestHandler<GetSlave2OrdersList, OrdersList>
    {
        private readonly ISlave2Service _service;

        public GetSlave2OrdersListQueryHandler(ISlave2Service service) => _service = service;

        public async Task<OrdersList> Handle(GetSlave2OrdersList request, CancellationToken cancellationToken) =>
            await _service.Get();
    }
}
