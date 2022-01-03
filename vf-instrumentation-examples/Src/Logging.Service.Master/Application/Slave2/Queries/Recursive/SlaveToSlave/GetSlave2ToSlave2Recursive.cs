using MediatR;

namespace Application.Slave2.Queries.Recursive.SlaveToSlave
{
    public class GetSlave2ToSlave2Recursive : IRequest<int>
    {
        public readonly int Number;

        public GetSlave2ToSlave2Recursive(int number) => Number = number;
    }
}
