﻿using System.Collections.Generic;

namespace Slave2.Entities
{
    public class Data
    {
        public int Id { get; set; }

        public ICollection<SubData> SubData { get; set; } = new List<SubData>();

        public string Value { get; set; }
    }
}