using System;

namespace Core.Models
{
    public class Message
    {
        public int Id { get; set; }
        public UserInfo Author { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}