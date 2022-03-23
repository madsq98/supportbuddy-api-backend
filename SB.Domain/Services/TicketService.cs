using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Core.IServices;
using Core.Models;
using SB.Domain.IRepositories;

namespace SB.Domain.Services
{
    public class TicketService : ITicketService
    {
        private const string
            InvalidSubject = DomainStrings.InvalidData + " Subject length must be over zero.",
            InvalidMessage = DomainStrings.InvalidData + " Message length must be over zero.",
            InvalidEmail = DomainStrings.InvalidData + " Email must be longer than zero and be correctly formatted.",
            InvalidFirstName = DomainStrings.InvalidData + " First Name length must be over zero",
            InvalidLastName = DomainStrings.InvalidData + " Last Name length must be over zero",
            InvalidPhoneNumber = DomainStrings.InvalidData + " Phone number length must be over zero and under nine.";
        
        private readonly I_RW_Repository<Ticket> _repo;
        private readonly I_RW_Repository<Answer> _answerRepo;

        public TicketService(I_RW_Repository<Ticket> repo, I_RW_Repository<Answer> answerRepo)
        {
            _repo = repo;
            _answerRepo = answerRepo;
        }
        
        public Ticket GetOneById(int id)
        {
            if (id > 0)
                return _repo.GetOneById(id);
            
            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _repo.GetAll();
        }

        public IEnumerable<Ticket> Search(string term)
        {
            if (term.Length > 0)
                return _repo.Search(term);

            throw new InvalidDataException(DomainStrings.TermLengthMustBeOverZero);
        }

        public Ticket Store(Ticket obj)
        {
            if (Validate(obj))
                return _repo.Insert(obj);

            return null;
        }

        public Ticket Update(Ticket obj)
        {
            if (Validate(obj))
                return _repo.Update(obj);

            return null;
        }

        public Ticket Delete(Ticket obj)
        {
            if (obj.Id > 0)
                return _repo.Delete(obj);

            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public bool Validate(Ticket obj)
        {
            if (obj.Subject.Length <= 0)
                throw new InvalidDataException(InvalidSubject);

            if (obj.Message.Length <= 0)
                throw new InvalidDataException(InvalidMessage);

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

        public Ticket AddAnswer(Ticket ticket, Answer answer)
        {
            if (answer.Message.Length <= 0)
                throw new InvalidDataException(InvalidMessage);


            var currentTicket = _repo.GetOneById(ticket.Id);
            answer.Author = new UserInfo {Id = currentTicket.UserInfo.Id};
            answer.TimeStamp = DateTime.Now;
            currentTicket.Answers.Add(answer);

            return _repo.Update(currentTicket);
        }

        public Ticket UpdateAnswer(Ticket ticket, Answer answer)
        {
            if (answer.Message.Length <= 0)
                throw new InvalidDataException(InvalidMessage);

            _answerRepo.Update(answer);

            return _repo.GetOneById(ticket.Id);
        }

        public Ticket DeleteAnswer(Ticket ticket, Answer answer)
        {
            if (answer.Id <= 0 || ticket.Id <= 0) 
                throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
            
            _answerRepo.Delete(answer);

            return _repo.GetOneById(ticket.Id);

        }
    }
}