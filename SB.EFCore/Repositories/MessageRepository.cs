using System.Collections.Generic;
using Core.Models;
using SB.Domain.IRepositories;

namespace SB.EFCore.Repositories
{
    public class MessageRepository : I_RW_Repository<Message>
    {
        public Message GetOneById(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Message> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Message> Search(string term)
        {
            throw new System.NotImplementedException();
        }

        public Message Insert(Message obj)
        {
            throw new System.NotImplementedException();
        }

        public Message Update(Message obj)
        {
            throw new System.NotImplementedException();
        }

        public Message Delete(Message obj)
        {
            throw new System.NotImplementedException();
        }
    }
}