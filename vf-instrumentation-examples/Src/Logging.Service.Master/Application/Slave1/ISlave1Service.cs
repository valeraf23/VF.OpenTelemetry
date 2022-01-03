using Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Slave1
{
    public interface ISlave1Service
    {
        Task<OrdersList> Get();
        Task<OrdersList> Parallel(string buyer);
        Task<DataList> GetData();
    }
}