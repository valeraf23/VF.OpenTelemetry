using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Master.Data.Queries.GetErrorTraces
{
    public class GetErrorTracesQuery : IRequest<List<UserDto>>
    {

        internal class GetErrorTracesHandler : IRequestHandler<GetErrorTracesQuery, List<UserDto>>
        {
            private readonly IMasterDbContext _db;

            public GetErrorTracesHandler(IMasterDbContext db) => _db = db;

            public async Task<List<UserDto>> Handle(GetErrorTracesQuery request, CancellationToken cancellationToken)
            {
                var range = Enumerable.Range(1, 10).ToList();
                var r = new Random();
                var i = r.Next(range.Count);
                if (i < 3)
                {
                    throw new Exception("Random Error_Traces");
                }

                return await _db.User.Select(data => new UserDto
                {
                    User = data.Value,
                    Message = data.SubData.Select(s => s.Value).ToList()
                }).ToListAsync(cancellationToken);
            }
        }
    }
}