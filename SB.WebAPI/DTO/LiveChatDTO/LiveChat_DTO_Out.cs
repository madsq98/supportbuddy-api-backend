using System;
using System.Collections.Generic;

namespace SB.WebAPI.DTO.LiveChatDTO
{
    public class LiveChat_DTO_Out
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }
        
        public int PhoneNumber { get; set; }
        
        public string Status { get; set; }
        
        public ICollection<Message_DTO_Out> Messages { get; set; }
    }
}