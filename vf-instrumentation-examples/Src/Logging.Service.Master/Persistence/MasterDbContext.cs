using Application.Common.Interfaces;
using Common;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence
{
    public class MasterDbContext : DbContext, IMasterDbContext, IDbContextSchema
    {
        public string Schema { get; }

        private readonly IDateTime _dateTime;

        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public MasterDbContext(DbContextOptions<MasterDbContext> options, IDateTime dateTime)
            : base(options) => _dateTime = dateTime;

        public DbSet<Order> Orders { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Message> Messages { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            AddConfiguration(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void AddConfiguration(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);
    }
}