using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Slave2.Queries.Recursive.SlaveToSlave
{
    public class GetSlave2ToSlave2RecursiveQueryHandler : IRequestHandler<GetSlave2ToSlave2Recursive, int>
    {
        private readonly ISlave2Service _services;

        public GetSlave2ToSlave2RecursiveQueryHandler(ISlave2Service services) => _services = services;

        public async Task<int> Handle(GetSlave2ToSlave2Recursive request, CancellationToken cancellationToken) =>
            await _services.SlaveToSlaveFactorial(request.Number);
    }
}
