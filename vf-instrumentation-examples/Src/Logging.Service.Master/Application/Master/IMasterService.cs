using System.Threading.Tasks;

namespace Application.Master
{
    public interface IMasterService
    {
        Task<int> RecursiveTraces(int count);
    }
}
