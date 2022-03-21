﻿namespace Core.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        
        public UserInfo UserInfo { get; set; }

        public string Subject { get; set; }
        
        public string Message { get; set; }
    }
}