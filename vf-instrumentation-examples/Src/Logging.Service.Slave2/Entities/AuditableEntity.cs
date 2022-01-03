using System;

namespace Slave2.Entities
{
    public class AuditableEntity
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}