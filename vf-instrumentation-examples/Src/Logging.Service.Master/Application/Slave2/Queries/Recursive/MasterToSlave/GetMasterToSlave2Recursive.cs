using MediatR;

namespace Application.Slave2.Queries.Recursive.MasterToSlave
{
    public class GetMasterToSlave2Recursive : IRequest<int>
    {
        public readonly int Number;

        public GetMasterToSlave2Recursive(int number) => Number = number;
    }
}
