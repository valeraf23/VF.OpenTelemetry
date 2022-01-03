using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");
            builder.Property(e => e.DeliveryStatus).IsRequired();
            builder.Property(e => e.Id).HasColumnName("OrderID");
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.BuyerName).IsRequired();
            builder.Property(e => e.Created);
            builder.Property(e => e.LastModified);
        }
    }
}