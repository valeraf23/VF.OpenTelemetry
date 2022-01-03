using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Slave2.Queries.Recursive.MasterToSlave
{
    public class GetMasterToSlave2RecursiveQueryHandler : IRequestHandler<GetMasterToSlave2Recursive, int>
    {
        private readonly ISlave2Service _services;

        public GetMasterToSlave2RecursiveQueryHandler(ISlave2Service services) => _services = services;

        public async Task<int> Handle(GetMasterToSlave2Recursive request, CancellationToken cancellationToken) =>
            await _services.MasterToSlaveFactorial(request.Number);
    }
}
