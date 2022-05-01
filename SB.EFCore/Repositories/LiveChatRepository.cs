using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class LiveChatRepository : I_RW_Repository<LiveChat>
    {
        private readonly SbContext _ctx;

        public LiveChatRepository(SbContext ctx)
        {
            _ctx = ctx;
        }
        public LiveChat GetOneById(int id)
        {
            return Conversion().FirstOrDefault(obj => obj.Id == id) ??
                   throw new FileNotFoundException(RepositoryStrings.IdNotFound);
        }

        public IEnumerable<LiveChat> GetAll()
        {
            return Conversion().ToList();
        }

        public IEnumerable<LiveChat> Search(string term)
        {
            return Conversion().Where(obj => obj.Author.FirstName.Contains(term) || obj.Author.LastName.Contains(term));
        }

        public LiveChat Insert(LiveChat obj)
        {
            var newEntity = _ctx.LiveChats.Add(new LiveChatEntity
            {
                Author = new UserInfoEntity
                {
                    Email = obj.Author.Email,
                    FirstName = obj.Author.FirstName,
                    LastName = obj.Author.LastName,
                    PhoneNumber = obj.Author.PhoneNumber
                },
                Messages = new List<MessageEntity>(),
                Open = true
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public LiveChat Update(LiveChat obj)
        {
            var currentEntity = GetOneById(obj.Id);

            if (obj.Messages == null || obj.Messages.Count == 0)
                obj.Messages = currentEntity.Messages;

            var newEntity = new LiveChatEntity
            {
                Id = obj.Id,
                AuthorId = obj.Author.Id,
                Open = obj.Open,
                Messages = obj.Messages.Select(msg => new MessageEntity
                {
                    Id = msg.Id,
                    AuthorId = msg.Author.Id,
                    Text = msg.Text,
                    Timestamp = msg.Timestamp
                }).ToList()
            };
            _ctx.ChangeTracker.Clear();
            _ctx.LiveChats.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public LiveChat Delete(LiveChat obj)
        {
            var entity = GetOneById(obj.Id);
            foreach (var msg in entity.Messages)
            {
                _ctx.Messages.Remove(new MessageEntity {Id = msg.Id});
            }

            _ctx.LiveChats.Remove(new LiveChatEntity {Id = obj.Id});
            _ctx.SaveChanges();

            return entity;
        }

        private IQueryable<LiveChat> Conversion()
        {
            return _ctx.LiveChats
                .Select(obj => new LiveChat
                {
                    Id = obj.Id,
                    Author = new UserInfo
                    {
                        Id = obj.Author.Id,
                        Email = obj.Author.Email,
                        FirstName = obj.Author.FirstName,
                        LastName = obj.Author.LastName,
                        PhoneNumber = obj.Author.PhoneNumber
                    },
                    Messages = obj.Messages.Select(msg => new Message
                    {
                        Id = msg.Id,
                        Text = msg.Text,
                        Timestamp = msg.Timestamp,
                        Author = new UserInfo
                        {
                            Id = msg.Author.Id,
                            Email = msg.Author.Email,
                            FirstName = msg.Author.FirstName,
                            LastName = msg.Author.LastName,
                            PhoneNumber = msg.Author.PhoneNumber
                        }
                    }).ToList(),
                    Open = obj.Open
                });
        }
    }
}