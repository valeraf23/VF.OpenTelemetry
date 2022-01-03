using Application.Common.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Decorators;

namespace Application.Master.Data.Queries.GetDataAll
{
    public class GetAllQuery : IRequest<List<UserDto>>
    {
        [Logging]
        //[RedisCache]
        internal class GetAllHandler : IRequestHandler<GetAllQuery, List<UserDto>>
        {
            private readonly IMasterDbContext _db;

            public GetAllHandler(IMasterDbContext db) => _db = db;

            public async Task<List<UserDto>> Handle(GetAllQuery request, CancellationToken cancellationToken) =>
                await _db.User.Select(data => new UserDto
                {
                    User = data.Value,
                    Message = data.SubData.Select(s => s.Value).ToList()
                }).ToListAsync(cancellationToken);
        }
    }
}