using Domain.ValueObjects;
using MediatR;

namespace Application.Master.Orders.Queries.ParallelToServices
{
    public class GetOrdersListFromParallel2Services : IRequest<OrdersList>
    {
    }
}
