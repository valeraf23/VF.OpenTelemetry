using Domain.ValueObjects;
using MediatR;

namespace Application.Slave1.Queries.SimpleGet
{
    public class GetSlave1OrdersList : IRequest<OrdersList> { }
}
