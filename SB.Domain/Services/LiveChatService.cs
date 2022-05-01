using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Core.IServices;
using Core.Models;
using SB.Domain.IRepositories;

namespace SB.Domain.Services
{
    public class LiveChatService : ILiveChatService
    {
        private const string
            InvalidSubject = DomainStrings.InvalidData + " Subject length must be over zero.",
            InvalidMessage = DomainStrings.InvalidData + " Message length must be over zero.",
            InvalidEmail = DomainStrings.InvalidData + " Email must be longer than zero and be correctly formatted.",
            InvalidFirstName = DomainStrings.InvalidData + " First Name length must be over zero",
            InvalidLastName = DomainStrings.InvalidData + " Last Name length must be over zero",
            InvalidPhoneNumber = DomainStrings.InvalidData + " Phone number length must be over zero and under nine.",
            StatusIsClosed = "It is not possible to update tickets with Status: Closed";
        
        private readonly I_RW_Repository<LiveChat> _repo;

        public LiveChatService(I_RW_Repository<LiveChat> repo)
        {
            _repo = repo;
        }

        public LiveChat GetOneById(int id)
        {
            if (id > 0)
                return _repo.GetOneById(id);
            
            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public IEnumerable<LiveChat> GetAll()
        {
            return _repo.GetAll();
        }

        public IEnumerable<LiveChat> Search(string term)
        {
            if (term.Length > 0)
                return _repo.Search(term);

            throw new InvalidDataException(DomainStrings.TermLengthMustBeOverZero);
        }

        public LiveChat Store(LiveChat obj)
        {
            return Validate(obj) ? _repo.Insert(obj) : null;
        }

        public LiveChat Update(LiveChat obj)
        {
            return Validate(obj) ? _repo.Update(obj) : null;
        }

        public LiveChat Delete(LiveChat obj)
        {
            if (obj.Id > 0)
                return _repo.Delete(obj);

            throw new InvalidDataException(DomainStrings.IdMustBeOverZero);
        }

        public bool Validate(LiveChat obj)
        {
            if (obj.Author.FirstName.Length <= 0)
                throw new InvalidDataException(InvalidFirstName);

            if (obj.Author.LastName.Length <= 0)
                throw new InvalidDataException(InvalidLastName);

            if (obj.Author.Email.Length <= 0 || !new EmailAddressAttribute().IsValid(obj.Author.Email))
                throw new InvalidDataException(InvalidEmail);
            
            if (obj.Author.PhoneNumber is not (> 0 and <= 999999999))
                throw new InvalidDataException(InvalidPhoneNumber);

            return true;
        }

        public LiveChat AddMessage(LiveChat liveChat, Message message)
        {
            if (message.Text.Length <= 0)
                throw new InvalidDataException(InvalidMessage);

            var currentLiveChat = _repo.GetOneById(liveChat.Id);

            if (!currentLiveChat.Open)
                throw new InvalidDataException(StatusIsClosed);

            message.Author = new UserInfo {Id = currentLiveChat.Author.Id};
            message.Timestamp = DateTime.Now;

            currentLiveChat.Messages.Add(message);

            return _repo.Update(currentLiveChat);
        }

        public LiveChat AddMessage(LiveChat liveChat, Message message, int supporterUserId)
        {
            if (message.Text.Length <= 0)
                throw new InvalidDataException(InvalidMessage);

            var currentLiveChat = _repo.GetOneById(liveChat.Id);

            if (!currentLiveChat.Open)
                throw new InvalidDataException(StatusIsClosed);

            message.Author = new UserInfo {Id = supporterUserId};
            message.Timestamp = DateTime.Now;

            currentLiveChat.Messages.Add(message);

            return _repo.Update(currentLiveChat);
        }

        public LiveChat CloseChat(LiveChat liveChat)
        {
            var currentLiveChat = _repo.GetOneById(liveChat.Id);

            if (!currentLiveChat.Open)
                throw new InvalidDataException(StatusIsClosed);

            currentLiveChat.Open = false;

            return _repo.Update(currentLiveChat);
        }
    }
}