using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Master.Data.Queries.GetNPlus1Traces
{
    public class GetNPlus1TracesQuery : IRequest<List<UserDto>>
    {
        internal class GetNPlus1TracesHandler : IRequestHandler<GetNPlus1TracesQuery, List<UserDto>>
        {
            private readonly IMasterDbContext _db;

            public GetNPlus1TracesHandler(IMasterDbContext db) => _db = db;

            public async Task<List<UserDto>> Handle(GetNPlus1TracesQuery request, CancellationToken cancellationToken)
            {
                var result = new List<UserDto>();
                var data = await _db.User.Where(x => x.Value.Contains("u")).ToListAsync(cancellationToken);

                foreach (var d in data)
                {
                    var sd = await _db.Messages.Where(y => y.UserId == d.Id && y.Value.Contains("a"))
                        .ToListAsync(cancellationToken);

                    var dto = new UserDto
                    {
                        User = d.Value,
                        Message = sd.Select(s => s.Value).ToList()
                    };
                    result.Add(dto);
                }

                return result;
            }
        }
    }
}