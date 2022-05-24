using System.IO;
using Core.Models;
using Moq;
using NUnit.Framework;
using SB.Domain.IRepositories;
using SB.Domain.Services;

namespace SB.CoreDomainTest
{
    [TestFixture]
    public class SupporterServiceTests
    {
        [Test]
        public void SupporterService_ThrowsInvalidDataException()
        {
            var mockRepoSupporters = new Mock<I_RW_Repository<Supporter>>();
            var supporterService = new SupporterService(mockRepoSupporters.Object);
            var supporter = new Supporter()
            {
                Username = "",
                Password = "",
                UserInfo = new UserInfo
                {
                    Email = "maildotcom",
                    FirstName = "",
                    LastName = "",
                    PhoneNumber = 1000000000
                }
            };
            Assert.Throws<InvalidDataException>(() => supporterService.Store(supporter));
        }

        [Test]
        public void SupporterService_ThrowsInvalidDataException_PhoneNumberError_WithMessage()
        {
            var mockRepoSupporters = new Mock<I_RW_Repository<Supporter>>();
            var supporterService = new SupporterService(mockRepoSupporters.Object);
            var supporter = new Supporter()
            {
                Username = "Test",
                Password = "test123",
                UserInfo = new UserInfo
                {
                    Email = "test@test.com",
                    FirstName = "Test",
                    LastName = "Testersen",
                    PhoneNumber = 1000000000
                }
            };
            var exception = Assert.Throws<InvalidDataException>(() => supporterService.Store(supporter));
            Assert.AreEqual("Invalid data supplied. Phone number length must be over zero and under nine.",
                exception.Message);
        }
    }
}