using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class TicketRepository : I_RW_Repository<Ticket>
    {
        private readonly SbContext _ctx;
        private readonly I_RW_Repository<UserInfo> _userInfoRepo;

        public TicketRepository(SbContext context)
        {
            _ctx = context;
            _userInfoRepo = new UserInfoRepository(context);
        }

        public Ticket GetOneById(int id)
        {
            return Conversion().FirstOrDefault(ticket => ticket.Id == id) ??
                   throw new FileNotFoundException(RepositoryStrings.IdNotFound);
        }

        public IEnumerable<Ticket> GetAll()
        {
            return Conversion().ToList();
        }

        public IEnumerable<Ticket> Search(string term)
        {
            return Conversion().Where(ticket => ticket.Subject.Contains(term)).ToList();
        }

        public Ticket Insert(Ticket obj)
        {
            var userInfoEntityId = (_userInfoRepo.Search(obj.UserInfo.Email).FirstOrDefault() ?? _userInfoRepo.Insert(obj.UserInfo)).Id;
            _userInfoRepo.Update(new UserInfo
            {
                Id = userInfoEntityId,
                Email = obj.UserInfo.Email,
                FirstName = obj.UserInfo.FirstName,
                LastName = obj.UserInfo.LastName,
                PhoneNumber = obj.UserInfo.PhoneNumber
            });

            var newEntity = _ctx.Tickets.Add(new TicketEntity
            {
                Subject = obj.Subject,
                Message = obj.Message,
                UserInfoId = userInfoEntityId,
                Status = 0,
                TimeStamp = DateTime.Now
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public Ticket Update(Ticket obj)
        {
            var currentEntity = GetOneById(obj.Id);
            obj.UserInfo.Id = currentEntity.UserInfo.Id;

            _userInfoRepo.Update(obj.UserInfo);

            if (obj.Answers == null || obj.Answers.Count == 0)
                obj.Answers = currentEntity.Answers;

            var newEntity = new TicketEntity
            {
                Id = obj.Id,
                Subject = obj.Subject,
                Message = obj.Message,
                UserInfoId = obj.UserInfo.Id,
                Status = (obj.Status == TicketStatus.Open) ? 0 : 1,
                Answers = obj.Answers.Select(answer =>
                {
                    var aEntity = new AnswerEntity
                    {
                        Id = answer.Id,
                        AuthorId = answer.Author.Id,
                        Message = answer.Message,
                        TimeStamp = answer.TimeStamp
                    };

                    if (answer.Attachment is {Id: > 0})
                    {
                        aEntity.AttachmentId = answer.Attachment.Id;
                    }

                    return aEntity;
                }).ToList(),
                TimeStamp = obj.TimeStamp
            };
            _ctx.ChangeTracker.Clear();
            _ctx.Tickets.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public Ticket Delete(Ticket obj)
        {
            var entity = GetOneById(obj.Id);
            foreach (var answer in entity.Answers)
            {
                _ctx.Answers.Remove(new AnswerEntity {Id = answer.Id});
            }
            _ctx.Tickets.Remove(new TicketEntity {Id = entity.Id});
            _ctx.SaveChanges();

            return entity;
        }

        private IQueryable<Ticket> Conversion()
        {
            return _ctx.Tickets
                .Include(ticket => ticket.UserInfo)
                .Select(ticket => new Ticket
                {
                    Id = ticket.Id,
                    Subject = ticket.Subject,
                    Message = ticket.Message,
                    UserInfo = new UserInfo
                    {
                        Id = ticket.UserInfo.Id,
                        Email = ticket.UserInfo.Email,
                        FirstName = ticket.UserInfo.FirstName,
                        LastName = ticket.UserInfo.LastName,
                        PhoneNumber = ticket.UserInfo.PhoneNumber
                    },
                    Status = (ticket.Status == 0) ? TicketStatus.Open : TicketStatus.Closed,
                    Answers = ticket.Answers.Select(answer => new Answer
                    {
                        Id = answer.Id,
                        Message = answer.Message,
                        Attachment = (answer.AttachmentId != null) ? new Attachment
                        {
                            Id = answer.AttachmentId ?? 0,
                            Url = answer.Attachment.Url
                        } : null,
                        TimeStamp = answer.TimeStamp,
                        Author = new UserInfo
                        {
                            Id = answer.Author.Id,
                            Email = answer.Author.Email,
                            FirstName = answer.Author.FirstName,
                            LastName = answer.Author.LastName,
                            PhoneNumber = answer.Author.PhoneNumber
                        }
                    }).ToList(),
                    TimeStamp = ticket.TimeStamp
                });
        }
    }
}