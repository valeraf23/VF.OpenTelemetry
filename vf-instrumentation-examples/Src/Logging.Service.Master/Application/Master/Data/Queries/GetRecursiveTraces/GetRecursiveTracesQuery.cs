using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Master.Data.Queries.GetRecursiveTraces
{
    public class GetRecursiveTracesQuery : IRequest<int>
    {
        public readonly int Counter;

        public GetRecursiveTracesQuery(int counter = 2) => Counter = counter;

        internal class GetRecursiveTracesHandler : IRequestHandler<GetRecursiveTracesQuery, int>
        {
            private readonly IMasterDbContext _db;
            private readonly IMasterService _service;

            public GetRecursiveTracesHandler(IMasterDbContext db, IMasterService service)
            {
                _db = db;
                _service = service;
            }

            public async Task<int> Handle(GetRecursiveTracesQuery request, CancellationToken cancellationToken)
            {
                var counter = request.Counter;
                var i = counter;
                while (counter > 0)
                {
                    counter -= 1;
                    var res = await _service.RecursiveTraces(counter - 1);
                    await _db.User.ToListAsync(cancellationToken);
                    i += res;
                }

                return i;
            }
        }
    }
}