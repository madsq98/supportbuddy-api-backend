using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Core.IServices;
using Core.Models;
using SB.Domain.IRepositories;

namespace SB.Domain.Services
{
    public class SupporterService : ISupporterService
    {
        private const int
            UsernameMinimumLength = 4,
            PasswordMinimumLength = 6;

        private readonly string
            InvalidUsername = DomainStrings.InvalidData + " Username length must be over " + UsernameMinimumLength +
                              " characters!",
            InvalidPassword = DomainStrings.InvalidData + " Password length must be over " + PasswordMinimumLength +
                              " characters!",
            InvalidEmail = DomainStrings.InvalidData + " Email must be longer than zero and be correctly formatted.",
            InvalidFirstName = DomainStrings.InvalidData + " First Name length must be over zero",
            InvalidLastName = DomainStrings.InvalidData + " Last Name length must be over zero",
            InvalidPhoneNumber = DomainStrings.InvalidData + " Phone number length must be over zero and under nine.",
            InvalidLogin = "Invalid username and/or password!";
        
        private readonly I_RW_Repository<Supporter> _repo;

        public SupporterService(I_RW_Repository<Supporter> repo)
        {
            _repo = repo;

            try
            {
                CheckLogin("testuser", "vpseliten123");
            }
            catch (InvalidDataException e)
            {
                Store(new Supporter
                {
                    Username = "testuser",
                    Password = "vpseliten123",
                    UserInfo = new UserInfo
                    {
                        Email = "test@test.com",
                        FirstName = "Test",
                        LastName = "Supporter",
                        PhoneNumber = 55224433
                    }
                });
            }
        }
        public Supporter GetOneById(int id)
        {
            if (id > 0)
                return _repo.GetOneById(id);

            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public IEnumerable<Supporter> GetAll()
        {
            return _repo.GetAll();
        }

        public IEnumerable<Supporter> Search(string term)
        {
            if (term.Length > 0)
                return _repo.Search(term);

            throw new InvalidDataException(DomainStrings.TermLengthMustBeOverZero);
        }

        public Supporter Store(Supporter obj)
        {
            return Validate(obj) ? _repo.Insert(obj) : null;
        }

        public Supporter Update(Supporter obj)
        {
            return Validate(obj) ? _repo.Update(obj) : null;
        }

        public Supporter Delete(Supporter obj)
        {
            if (obj.Id > 0)
                return _repo.Delete(obj);

            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public bool Validate(Supporter obj)
        {
            if (obj.Username.Length < UsernameMinimumLength)
                throw new InvalidDataException(InvalidUsername);

            if (obj.Password.Length < PasswordMinimumLength)
                throw new InvalidDataException(InvalidPassword);
            
            if (obj.UserInfo.Email.Length <= 0 || !new EmailAddressAttribute().IsValid(obj.UserInfo.Email))
                throw new InvalidDataException(InvalidEmail);
            
            if (obj.UserInfo.FirstName.Length <= 0)
                throw new InvalidDataException(InvalidFirstName);

            if (obj.UserInfo.LastName.Length <= 0)
                throw new InvalidDataException(InvalidLastName);

            if (obj.UserInfo.PhoneNumber is not (> 0 and <= 999999999))
                throw new InvalidDataException(InvalidPhoneNumber);

            return true;
        }

        public int CheckLogin(string username, string password)
        {
            List<Supporter> objList = _repo.Search(username).ToList();
            if (objList.Count <= 0)
                throw new InvalidDataException(InvalidLogin);

            if (objList[0].Password != password)
                throw new InvalidDataException(InvalidLogin);

            return objList[0].Id;
        }
    }
}