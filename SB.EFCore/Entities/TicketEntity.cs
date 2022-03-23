using System;
using System.Collections.Generic;

namespace SB.EFCore.Entities
{
    public class TicketEntity
    {
        public int Id { get; set; }
        
        public UserInfoEntity UserInfo { get; set; }
        
        public int UserInfoId { get; set; }
        
        public string Subject { get; set; }
        
        public string Message { get; set; }
        
        public IEnumerable<AnswerEntity> Answers { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}