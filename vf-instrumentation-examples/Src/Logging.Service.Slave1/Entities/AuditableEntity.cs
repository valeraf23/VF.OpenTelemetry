﻿using System;

namespace Slave1.Entities
{
    public class AuditableEntity
    {
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}