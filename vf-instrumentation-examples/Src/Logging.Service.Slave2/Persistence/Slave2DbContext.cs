using Microsoft.EntityFrameworkCore;
using Slave2.Entities;

namespace Slave2.Persistence
{
    public class Slave2DbContext : DbContext
    {
        public Slave2DbContext(DbContextOptions<Slave2DbContext> options):base(options) { }

        public DbSet<Data> Datas { get; set; }
        public DbSet<SubData> SubDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dataBuilder = modelBuilder.Entity<Data>();
            dataBuilder.HasKey(e => e.Id).HasName("ID");
            dataBuilder.Property(e => e.Value).IsRequired().HasMaxLength(50);

            var subDataBuilder = modelBuilder.Entity<SubData>();
            subDataBuilder.Property(e => e.Id).HasColumnName("ID");
            subDataBuilder.Property(e => e.Value).IsRequired().HasMaxLength(50);
            subDataBuilder.HasOne(p => p.Data)
                .WithMany(b => b.SubData)
                .HasPrincipalKey(d => d.Id)
                .HasForeignKey(d => d.DataId)
                .HasConstraintName("FK_DATA_ID");
            base.OnModelCreating(modelBuilder);
        }
    }
}