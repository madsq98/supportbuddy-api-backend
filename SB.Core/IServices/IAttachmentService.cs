using Core.Models;

namespace Core.IServices
{
    public interface IAttachmentService
    {
        public Attachment Store(Attachment obj);
    }
}