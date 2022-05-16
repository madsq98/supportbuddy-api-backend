using System;
using Core.Models;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class AttachmentRepository : I_W_Repository<Attachment>
    {
        private readonly SbContext _ctx;

        public AttachmentRepository(SbContext ctx)
        {
            _ctx = ctx;
        }
        public Attachment Insert(Attachment obj)
        {
            var newEntity = _ctx.Attachments.Add(new AttachmentEntity
            {
                Url = obj.Url
            }).Entity;

            _ctx.SaveChanges();
            obj.Id = newEntity.Id;

            return obj;
        }

        public Attachment Update(Attachment obj)
        {
            throw new System.NotImplementedException();
        }

        public Attachment Delete(Attachment obj)
        {
            throw new System.NotImplementedException();
        }
    }
}