using Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Domain.Extensions
{
    public static class ValueObjectExtension
    {
        public static OrdersList ToOrderList(this IEnumerable<Order> orders)
        {
            var ol = new OrdersList
            {
                Orders = orders.ToList()
            };

            return ol;
        }

        public static DataList ToDataList(this IEnumerable<User> data)
        {
            var ol = new DataList
            {
                Data = data.ToList()
            };

            return ol;
        }
    }
}
