using System.Collections.Generic;

namespace Core.Models
{
    public class LiveChat
    {
        public int Id { get; set; }
        public UserInfo Author { get; set; }
        public ICollection<Message> Messages { get; set; }
        public bool Open { get; set; }
    }
}