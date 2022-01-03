using CSharpFunctionalExtensions;
using Domain.Entities;
using Domain.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Domain.ValueObjects
{
    public class DataList : ValueObject
    {
        public List<User> Data { get; set; }

        protected override IEnumerable<object> GetEqualityComponents() => Data;

        public static DataList operator +(DataList a, DataList b)
        {
            var list = a.Data.Concat(b.Data);
            return list.ToDataList();
        }

        public static DataList operator -(DataList a, DataList b)
        {
            var list = a.Data.Except(b.Data);
            return list.ToDataList();
        }
    }
}