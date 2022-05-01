using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;

namespace SB.WebAPI.Util
{
    public interface IWebsocketHandler
    {
        public Task Handle(Guid id, WebSocket webSocket, ILiveChatService service, LiveChat liveChat, Supporter supporter);
    }
}