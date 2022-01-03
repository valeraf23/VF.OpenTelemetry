using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMasterDbContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<User> User { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}