using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        
        public UserInfo UserInfo { get; set; }
        
        public TicketStatus Status { get; set; }

        public string Subject { get; set; }
        
        public string Message { get; set; }
        
        public ICollection<Answer> Answers { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}