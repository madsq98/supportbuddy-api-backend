using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class AnswerRepository : I_RW_Repository<Answer>
    {
        private readonly SbContext _ctx;
        private readonly I_RW_Repository<UserInfo> _userInfoRepo;

        public AnswerRepository(SbContext context)
        {
            _ctx = context;
            _userInfoRepo = new UserInfoRepository(context);
        }
        public Answer GetOneById(int id)
        {
            return Conversion().FirstOrDefault(answer => answer.Id == id) ??
                   throw new FileNotFoundException(RepositoryStrings.IdNotFound);
        }

        public IEnumerable<Answer> GetAll()
        {
            return Conversion().ToList();
        }

        public IEnumerable<Answer> Search(string term)
        {
            return Conversion().Where(answer => answer.Message.Contains(term)).ToList();
        }

        public Answer Insert(Answer obj)
        {
            var userInfoEntityId = obj.Author.Id;

            var newEntity = _ctx.Answers.Add(new AnswerEntity
            {
                AuthorId = userInfoEntityId,
                Message = obj.Message,
                TimeStamp = DateTime.Now
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public Answer Update(Answer obj)
        {
            var currentEntity = GetOneById(obj.Id);
            obj.Author.Id = currentEntity.Author.Id;

            var newEntity = new AnswerEntity
            {
                Id = obj.Id,
                AuthorId = obj.Author.Id,
                Message = obj.Message,
                TimeStamp = currentEntity.TimeStamp
            };
            _ctx.ChangeTracker.Clear();
            _ctx.Answers.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public Answer Delete(Answer obj)
        {
            var entity = GetOneById(obj.Id);
            _ctx.Answers.Remove(new AnswerEntity {Id = obj.Id});
            _ctx.SaveChanges();

            return entity;
        }

        private IQueryable<Answer> Conversion()
        {
            return _ctx.Answers
                .Select(answer => new Answer
                {
                    Id = answer.Id,
                    Author = new UserInfo
                    {
                        Id = answer.Author.Id,
                        Email = answer.Author.Email,
                        FirstName = answer.Author.FirstName,
                        LastName = answer.Author.LastName,
                        PhoneNumber = answer.Author.PhoneNumber
                    },
                    Message = answer.Message,
                    TimeStamp = answer.TimeStamp
                });
        }
    }
}