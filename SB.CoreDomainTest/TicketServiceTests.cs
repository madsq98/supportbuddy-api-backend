using System.IO;
using Core.IServices;
using Core.Models;
using Moq;
using NUnit.Framework;
using SB.Domain.IRepositories;
using SB.Domain.Services;
using SB.EFCore.Repositories;

namespace SB.CoreDomainTest
{
    [TestFixture]
    public class TicketServiceTests
    {
        [Test]
        public void TicketService_ThrowsInvalidDataException()
        {
            var mockRepoTickets = new Mock<I_RW_Repository<Ticket>>();
            var mockRepoAnswers = new Mock<I_RW_Repository<Answer>>();
            var ticketService = new TicketService(mockRepoTickets.Object,mockRepoAnswers.Object);
            var ticket = new Ticket
            {
                Message = "",
                Subject = "",
                UserInfo = new UserInfo
                {
                    Email = "maildotcom",
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = 1000000000
                }
            };
            Assert.Throws<InvalidDataException>(() => ticketService.Store(ticket));
        }
        [Test]
        public void TicketService_ThrowsInvalidDataException_PhoneNumberError_WithMessage()
        {
            var mockRepoTickets = new Mock<I_RW_Repository<Ticket>>();
            var mockRepoAnswers = new Mock<I_RW_Repository<Answer>>();
            var ticketService = new TicketService(mockRepoTickets.Object,mockRepoAnswers.Object);
            var ticket = new Ticket
            {
                Message = "Bobby",
                Subject = "Bobsen",
                UserInfo = new UserInfo
                {
                    Email = "test@test.com",
                    FirstName = "Bobby",
                    LastName = "Bobsen",
                    PhoneNumber = 1000000000
                }
            };
            var exception = Assert.Throws<InvalidDataException>(() => ticketService.Store(ticket));
            Assert.AreEqual("Invalid data supplied. Phone number length must be over zero and under nine.",exception.Message);
        }
    }
}