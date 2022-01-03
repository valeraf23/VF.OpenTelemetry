using Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Slave2
{
    public interface ISlave2Service
    {
        Task<OrdersList> Get();
        Task<int> SlaveToSlaveFactorial(int number);
        Task<int> MasterToSlaveFactorial(int number);
        Task<DataList> GetData();
    }
}