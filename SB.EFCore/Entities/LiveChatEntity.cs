using System.Collections.Generic;

namespace SB.EFCore.Entities
{
    public class LiveChatEntity
    {
        public int Id { get; set; }
        public UserInfoEntity Author { get; set; }
        
        public int AuthorId { get; set; }
        public ICollection<MessageEntity> Messages { get; set; }
        public bool Open { get; set; }
    }
}