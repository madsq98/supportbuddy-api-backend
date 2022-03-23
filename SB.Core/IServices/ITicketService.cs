using System.Collections.Generic;
using Core.Models;

namespace Core.IServices
{
    public interface ITicketService : I_RW_Service<Ticket>
    {
        public Ticket AddAnswer(Ticket ticket, Answer answer);

        public Ticket UpdateAnswer(Ticket ticket, Answer answer);

        public Ticket DeleteAnswer(Ticket ticket, Answer answer);
    }
}