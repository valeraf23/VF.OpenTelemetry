using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.System.Commands.SeedSampleData
{
    public class SampleDataSeeder
    {
        private readonly IMasterDbContext _context;

        public SampleDataSeeder(IMasterDbContext context) => _context = context;

        public async Task SeedAllAsync(CancellationToken cancellationToken) =>
            // await SeedOrders(cancellationToken);
            await SeedUsers(cancellationToken);

        public async Task SeedOrders(CancellationToken cancellationToken)
        {
            if (_context.Orders.Any()) return;
            var orders = new List<Order>
            {
                new()
                {
                    BuyerName = "VF",
                    DeliveryStatus = DeliveryStatus.Completed,
                    IsBundle = true,
                    Title = "First Example",
                    Id = Guid.NewGuid(),
                    Created = DateTime.Today.AddDays(-1),
                    LastModified = DateTime.Today.AddHours(-1)
                }
            };

            await _context.Orders.AddRangeAsync(orders, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SeedUsers(CancellationToken cancellationToken)
        {
            if (_context.Messages.Any()) return;
            var data = Enumerable.Range(1, 20).Select(i =>
                new User
                {
                    Value = $"user {i}",
                    SubData = GetSubData()
                }
            );

            if (_context.Messages.Any()) return;

            await _context.User.AddRangeAsync(data, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static ICollection<Message> GetSubData()
        {
            var subData = Enumerable.Range(1, 20).Select(i =>
                new Message
                {
                    Value = $"message {i}"
                }
            );
            return subData.ToList();
        }
    }
}