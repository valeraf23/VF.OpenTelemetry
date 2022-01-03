using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class SubDataConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.UserId).HasColumnName("USER_ID");
            builder.Property(e => e.Value).IsRequired().HasMaxLength(50);
            builder.HasOne(p => p.User)
                .WithMany(b => b.SubData)
                .HasPrincipalKey(d => d.Id)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_USER_ID");
        }
    }
}