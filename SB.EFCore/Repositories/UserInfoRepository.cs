using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using SB.Domain.IRepositories;
using SB.EFCore.Entities;

namespace SB.EFCore.Repositories
{
    public class UserInfoRepository : I_RW_Repository<UserInfo>
    {
        private readonly SbContext _ctx;

        public UserInfoRepository(SbContext context)
        {
            _ctx = context;
        }
        public UserInfo GetOneById(int id)
        {
            return Conversion().FirstOrDefault(info => info.Id == id) ??
                   throw new FileNotFoundException(RepositoryStrings.IdNotFound);
        }

        public IEnumerable<UserInfo> GetAll()
        {
            return Conversion().ToList();
        }

        public IEnumerable<UserInfo> Search(string term)
        {
            return Conversion().Where(info => info.Email == term).ToList();
        }

        public UserInfo Insert(UserInfo obj)
        {
            var newEntity = _ctx.UserInfoEntities.Add(new UserInfoEntity
            {
                Email = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                PhoneNumber = obj.PhoneNumber
            }).Entity;
            _ctx.SaveChanges();

            return GetOneById(newEntity.Id);
        }

        public UserInfo Update(UserInfo obj)
        {
            GetOneById(obj.Id);

            var newEntity = new UserInfoEntity
            {
                Id = obj.Id,
                Email = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                PhoneNumber = obj.PhoneNumber
            };
            _ctx.ChangeTracker.Clear();
            _ctx.UserInfoEntities.Update(newEntity);
            _ctx.SaveChanges();

            return GetOneById(obj.Id);
        }

        public UserInfo Delete(UserInfo obj)
        {
            var entity = GetOneById(obj.Id);
            _ctx.UserInfoEntities.Remove(new UserInfoEntity {Id = obj.Id});
            _ctx.SaveChanges();

            return entity;
        }

        private IQueryable<UserInfo> Conversion()
        {
            return _ctx.UserInfoEntities
                .Select(info => new UserInfo
                {
                   Id = info.Id,
                   FirstName = info.FirstName,
                   LastName = info.LastName,
                   Email = info.Email,
                   PhoneNumber = info.PhoneNumber
                });
        }
    }
}