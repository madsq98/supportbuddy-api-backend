using System;
using System.Net.WebSockets;
using Core.Models;

namespace SB.WebAPI.Util
{
    public class SocketConnection
    {
        public Guid Id { get; set; }
        
        public WebSocket WebSocket { get; set; }
        
        public LiveChat LiveChat { get; set; }
        
        public Supporter Supporter { get; set; }
    }
}