using System;
using Domain.Entities;

namespace Domain.Common
{
    public class AuditableEntity : BaseModel<Guid>
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}