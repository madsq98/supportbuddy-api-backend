using System;

namespace Core.Models
{
    public class Answer
    {
        public int Id { get; set; }
        
        public UserInfo Author { get; set; }
        
        public string Message { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}