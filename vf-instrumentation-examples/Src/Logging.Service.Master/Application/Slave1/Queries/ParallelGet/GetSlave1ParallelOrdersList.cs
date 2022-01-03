using Domain.ValueObjects;
using MediatR;

namespace Application.Slave1.Queries.ParallelGet
{
    public class GetSlave1ParallelOrdersList : IRequest<OrdersList>
    {
        public readonly string Buyer;

        public GetSlave1ParallelOrdersList(string buyer) => Buyer = buyer;

    }
}
