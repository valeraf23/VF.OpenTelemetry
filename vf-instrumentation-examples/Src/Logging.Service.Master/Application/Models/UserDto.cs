using System.Collections.Generic;

namespace Application.Models
{
    public class UserDto
    {
        public string User { get; set; }
        public ICollection<string> Message { get; set; }
    }
}