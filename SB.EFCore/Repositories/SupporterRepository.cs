using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class SupporterRepository : I_RW_Repository<Supporter>
    {
        private readonly SbContext _ctx;
        private readonly I_RW_Repository<UserInfo> _userInfoRepo;

        public SupporterRepository(SbContext context)
        {
            _ctx = context;
            _userInfoRepo = new UserInfoRepository(context);
        }
        public Supporter GetOneById(int id)
        {
            return Conversion().FirstOrDefault(supporter => supporter.Id == id) ??
                   throw new FileNotFoundException(RepositoryStrings.IdNotFound);
        }

        public IEnumerable<Supporter> GetAll()
        {
            return Conversion().ToList();
        }

        public IEnumerable<Supporter> Search(string term)
        {
            //Searching in username field, and it must be equal to term
            return Conversion().Where(supporter => supporter.Username == term).ToList();
        }

        public Supporter Insert(Supporter obj)
        {
            var newUserInfo = _userInfoRepo.Insert(obj.UserInfo);

            var newEntity = _ctx.Supporters.Add(new SupporterEntity
            {
                Username = obj.Username,
                Password = obj.Password,
                UserInfoId = newUserInfo.Id
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public Supporter Update(Supporter obj)
        {
            var currentInfoEntity = _userInfoRepo.GetOneById(obj.UserInfo.Id);

            _userInfoRepo.Update(obj.UserInfo);

            var newEntity = new SupporterEntity
            {
                Username = obj.Username,
                Password = obj.Password,
                UserInfoId = currentInfoEntity.Id
            };
            _ctx.ChangeTracker.Clear();
            _ctx.Supporters.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public Supporter Delete(Supporter obj)
        {
            var entity = GetOneById(obj.Id);
            
            _ctx.Supporters.Remove(new SupporterEntity {Id = obj.Id});
            _ctx.SaveChanges();

            return entity;
        }

        private IQueryable<Supporter> Conversion()
        {
            return _ctx.Supporters
                .Include(supporter => supporter.UserInfo)
                .Select(supporter => new Supporter
                {
                    Id = supporter.Id,
                    Username = supporter.Username,
                    Password = supporter.Password,
                    UserInfo = new UserInfo
                    {
                        Id = supporter.UserInfo.Id,
                        FirstName = supporter.UserInfo.FirstName,
                        LastName = supporter.UserInfo.LastName,
                        Email = supporter.UserInfo.Email,
                        PhoneNumber = supporter.UserInfo.PhoneNumber
                    }
                });
        }
    }
}