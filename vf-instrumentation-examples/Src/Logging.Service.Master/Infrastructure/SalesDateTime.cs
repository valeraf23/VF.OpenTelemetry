using System;
using Common;

namespace Infrastructure
{
    public class SalesDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}