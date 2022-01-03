using Domain.ValueObjects;
using MediatR;

namespace Application.Slave2.Queries.SimpleGet
{
    public class GetSlave2OrdersList : IRequest<OrdersList> { }
}
