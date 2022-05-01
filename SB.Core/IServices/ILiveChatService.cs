using Core.Models;

namespace Core.IServices
{
    public interface ILiveChatService : I_RW_Service<LiveChat>
    {
        public LiveChat AddMessage(LiveChat liveChat, Message message);

        public LiveChat AddMessage(LiveChat liveChat, Message message, int supporterUserId);

        public LiveChat CloseChat(LiveChat liveChat);
    }
}