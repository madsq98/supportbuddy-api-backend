using System;

namespace SB.EFCore.Entities
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public UserInfoEntity Author { get; set; }
        
        public int AuthorId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}