using CSharpFunctionalExtensions;
using System.Collections.Generic;
using Domain.Entities;

namespace Domain.ValueObjects
{
    public class OrdersList : ValueObject
    {
        public List<Order> Orders { get; set; }

        protected override IEnumerable<object> GetEqualityComponents() => Orders;

    }
}
