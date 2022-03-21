using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;
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
                UserInfoId = userInfoEntityId
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public Ticket Update(Ticket obj)
        {
            var currentEntity = GetOneById(obj.Id);
            obj.UserInfo.Id = currentEntity.UserInfo.Id;

            _userInfoRepo.Update(obj.UserInfo);
            
            var newEntity = new TicketEntity
            {
                Id = obj.Id,
                Subject = obj.Subject,
                Message = obj.Message,
                UserInfoId = obj.UserInfo.Id
            };
            _ctx.ChangeTracker.Clear();
            _ctx.Tickets.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public Ticket Delete(Ticket obj)
        {
            var entity = GetOneById(obj.Id);
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
                    }
                });
        }
    }
}