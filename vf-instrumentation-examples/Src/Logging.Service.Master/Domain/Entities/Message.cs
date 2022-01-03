using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("message")]
    public class Message: BaseModel<int>
    {
        public string Value { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}