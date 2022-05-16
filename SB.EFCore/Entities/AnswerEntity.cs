using System;

namespace SB.EFCore.Entities
{
    public class AnswerEntity
    {
        public int Id { get; set; }
        
        public UserInfoEntity Author { get; set; }
        
        public int AuthorId { get; set; }
        
        public string Message { get; set; }
        
        public int? AttachmentId { get; set; }
        
        public AttachmentEntity? Attachment { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}