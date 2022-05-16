using System.IO;
using Core.IServices;
using Core.Models;
using SB.Domain.IRepositories;

namespace SB.Domain.Services
{
    public class AttachmentService : IAttachmentService
    {
        private const string InvalidUrl = DomainStrings.InvalidData + " Attachment URL length must be over zero!";
        
        private readonly I_W_Repository<Attachment> _repo;

        public AttachmentService(I_W_Repository<Attachment> repo)
        {
            _repo = repo;
        }

        public Attachment Store(Attachment obj)
        {
            Validate(obj);

            return _repo.Insert(obj);
        }

        private void Validate(Attachment obj)
        {
            if (obj.Url.Length <= 0)
                throw new InvalidDataException(InvalidUrl);
        }
    }
}