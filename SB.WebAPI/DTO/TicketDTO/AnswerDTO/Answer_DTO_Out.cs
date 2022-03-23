using System;

namespace SB.WebAPI.DTO.TicketDTO.AnswerDTO
{
    public class Answer_DTO_Out
    {
        public int Id { get; set; }
        
        public string AuthorFirstName { get; set; }
        
        public string AuthorLastName { get; set; }
        
        public string Message { get; set; }
        
        public DateTime TimeStamp { get; set; }
    }
}