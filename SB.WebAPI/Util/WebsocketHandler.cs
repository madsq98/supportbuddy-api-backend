using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;

namespace SB.WebAPI.Util
{
    public class WebsocketHandler : IWebsocketHandler
    {
        public WebsocketHandler()
        {
            SetupCleanUpTask();
        }
        
        public List<SocketConnection> websocketConnections = new List<SocketConnection>();

        public async Task Handle(Guid id, WebSocket webSocket, ILiveChatService service, LiveChat liveChat, Supporter supporter = null)
        {
            lock (websocketConnections)
            {
                websocketConnections.Add(new SocketConnection
                {
                    Id = id,
                    WebSocket = webSocket,
                    LiveChat = liveChat,
                    Supporter = supporter
                });
            }

            if (supporter != null)
                await SendMessageToSockets(
                    $"{supporter.UserInfo.FirstName} {supporter.UserInfo.LastName} has joined the session!", liveChat);
            else
                await SendMessageToSockets(
                    $"{liveChat.Author.FirstName} {liveChat.Author.LastName} has joined the session!", liveChat);

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(id, webSocket, service, liveChat, supporter);
                if (message != null)
                    await SendMessageToSockets(message, liveChat);
            }
        }

        private async Task<string> ReceiveMessage(Guid id, WebSocket webSocket, ILiveChatService service, LiveChat liveChat, Supporter supporter)
        {
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);

            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');

                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (supporter != null)
                    {
                        service.AddMessage(liveChat, new Message {Text = message}, supporter.Id);
                        return $"{supporter.UserInfo.FirstName} {supporter.UserInfo.LastName}: {message}";
                    }
                    else
                    {
                        service.AddMessage(liveChat, new Message {Text = message});
                        return $"{liveChat.Author.FirstName} {liveChat.Author.LastName}: {message}";
                    }
                }
            }
            return null;
        }

        private async Task SendMessageToSockets(string message, LiveChat liveChat)
        {
            IEnumerable<SocketConnection> toSendTo;

            lock (websocketConnections)
            {
                toSendTo = websocketConnections.ToList();
            }

            var tasks = toSendTo.Select(async websocketConnection =>
            {
                if (websocketConnection.LiveChat.Id == liveChat.Id)
                {
                    var bytes = Encoding.Default.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes);

                    await websocketConnection.WebSocket.SendAsync(
                        arraySegment, 
                        WebSocketMessageType.Text, 
                        true,
                        CancellationToken.None);
                }
            });

            await Task.WhenAll(tasks);
        }
        
        private void SetupCleanUpTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    IEnumerable<SocketConnection> openSockets;
                    IEnumerable<SocketConnection> closedSockets;

                    lock (websocketConnections)
                    {
                        openSockets = websocketConnections.Where(x => x.WebSocket.State == WebSocketState.Open || x.WebSocket.State == WebSocketState.Connecting);
                        closedSockets = websocketConnections.Where(x => x.WebSocket.State != WebSocketState.Open && x.WebSocket.State != WebSocketState.Connecting);

                        websocketConnections = openSockets.ToList();
                    }

                    foreach (var closedWebsocketConnection in closedSockets)
                    {
                        string firstName;
                        string lastName;
                        if (closedWebsocketConnection.Supporter != null)
                        {
                            firstName = closedWebsocketConnection.Supporter.UserInfo.FirstName;
                            lastName = closedWebsocketConnection.Supporter.UserInfo.LastName;
                        }
                        else
                        {
                            firstName = closedWebsocketConnection.LiveChat.Author.FirstName;
                            lastName = closedWebsocketConnection.LiveChat.Author.LastName;
                        }

                        await SendMessageToSockets($"{firstName} {lastName} has left the chat", closedWebsocketConnection.LiveChat);
                    }
            
                    await Task.Delay(5000);
                }    
            });
        }
    }
}