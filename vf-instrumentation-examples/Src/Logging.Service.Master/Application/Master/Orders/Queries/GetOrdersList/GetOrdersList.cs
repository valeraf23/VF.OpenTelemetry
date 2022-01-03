using Domain.ValueObjects;
using MediatR;

namespace Application.Master.Orders.Queries.GetOrdersList
{
    public class GetOrdersList : IRequest<OrdersList>
    {
    }
}
