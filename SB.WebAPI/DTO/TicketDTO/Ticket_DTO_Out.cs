using System;
using System.Collections.Generic;
using SB.WebAPI.DTO.TicketDTO.AnswerDTO;

namespace SB.WebAPI.DTO.TicketDTO
{
    public class Ticket_DTO_Out
    {
        public int Id { get; set; }
        
        public string Subject { get; set; }
        
        public string Message { get; set; }
        
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int PhoneNumber { get; set; }
        
        public ICollection<Answer_DTO_Out> Answers { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}