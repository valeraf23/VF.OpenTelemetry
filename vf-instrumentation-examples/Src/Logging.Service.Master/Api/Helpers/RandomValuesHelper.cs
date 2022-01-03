using System;
using System.Linq;

namespace Master.Helpers
{
    public static class RandomValuesHelper
    {
        public static string RandomString(int length = 40)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ012345678";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}