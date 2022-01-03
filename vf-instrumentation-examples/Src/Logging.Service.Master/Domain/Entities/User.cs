using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("user")]
    public class User : BaseModel<int>
    {
        public ICollection<Message> SubData { get; set; } = new List<Message>();

        public string Value { get; set; }
    }
}