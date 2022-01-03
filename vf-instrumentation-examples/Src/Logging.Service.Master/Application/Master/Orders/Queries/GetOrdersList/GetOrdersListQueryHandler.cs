using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Extensions;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Master.Orders.Queries.GetOrdersList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersList, OrdersList>
    {
        private readonly IMasterDbContext _context;

        public GetOrdersListQueryHandler(IMasterDbContext context) => _context = context;

        public async Task<OrdersList> Handle(GetOrdersList request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .ToListAsync(cancellationToken);
            return orders.ToOrderList();
        }
    }
}
