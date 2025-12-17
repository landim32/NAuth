using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class UserPhoneRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly UserPhoneRepository _repository;
        private readonly Mock<IUserPhoneDomainFactory> _mockFactory;
        private readonly Mock<IUserPhoneModel> _mockPhoneModel;

        public UserPhoneRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new UserPhoneRepository(_context);
            _mockFactory = new Mock<IUserPhoneDomainFactory>();
            _mockPhoneModel = new Mock<IUserPhoneModel>();

            _mockFactory.Setup(f => f.BuildUserPhoneModel()).Returns(_mockPhoneModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #region Insert Tests

        [Fact]
        public void Insert_WithValidPhone_ShouldInsertSuccessfully()
        {
            // Arrange
            _mockPhoneModel.SetupGet(m => m.PhoneId).Returns(0);
            _mockPhoneModel.SetupGet(m => m.UserId).Returns(1L);
            _mockPhoneModel.SetupGet(m => m.Phone).Returns("1234567890");
            _mockPhoneModel.SetupSet(m => m.PhoneId = It.IsAny<long>());

            // Act
            var result = _repository.Insert(_mockPhoneModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedPhone = _context.UserPhones.FirstOrDefault(p => p.UserId == 1L);
            Assert.NotNull(savedPhone);
            Assert.Equal(1L, savedPhone.UserId);
            Assert.Equal("1234567890", savedPhone.Phone);
            _mockPhoneModel.VerifySet(m => m.PhoneId = It.IsAny<long>(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldGeneratePhoneId()
        {
            // Arrange
            _mockPhoneModel.SetupGet(m => m.PhoneId).Returns(0);
            _mockPhoneModel.SetupGet(m => m.UserId).Returns(1L);
            _mockPhoneModel.SetupGet(m => m.Phone).Returns("9876543210");
            _mockPhoneModel.SetupSet(m => m.PhoneId = It.IsAny<long>());

            // Act
            _repository.Insert(_mockPhoneModel.Object, _mockFactory.Object);

            // Assert
            var savedPhone = _context.UserPhones.FirstOrDefault();
            Assert.NotNull(savedPhone);
            Assert.True(savedPhone.PhoneId > 0);
        }

        [Fact]
        public void Insert_MultiplePhones_ShouldInsertAll()
        {
            // Arrange
            var mockPhone1 = new Mock<IUserPhoneModel>();
            mockPhone1.SetupGet(m => m.UserId).Returns(1L);
            mockPhone1.SetupGet(m => m.Phone).Returns("1111111111");

            var mockPhone2 = new Mock<IUserPhoneModel>();
            mockPhone2.SetupGet(m => m.UserId).Returns(1L);
            mockPhone2.SetupGet(m => m.Phone).Returns("2222222222");

            // Act
            _repository.Insert(mockPhone1.Object, _mockFactory.Object);
            _repository.Insert(mockPhone2.Object, _mockFactory.Object);

            // Assert
            Assert.Equal(2, _context.UserPhones.Count());
        }

        [Fact]
        public void Insert_MultiplePhonesForDifferentUsers_ShouldInsertAll()
        {
            // Arrange
            var mockPhone1 = new Mock<IUserPhoneModel>();
            mockPhone1.SetupGet(m => m.UserId).Returns(1L);
            mockPhone1.SetupGet(m => m.Phone).Returns("1111111111");

            var mockPhone2 = new Mock<IUserPhoneModel>();
            mockPhone2.SetupGet(m => m.UserId).Returns(2L);
            mockPhone2.SetupGet(m => m.Phone).Returns("2222222222");

            // Act
            _repository.Insert(mockPhone1.Object, _mockFactory.Object);
            _repository.Insert(mockPhone2.Object, _mockFactory.Object);

            // Assert
            var user1Phones = _context.UserPhones.Where(p => p.UserId == 1L).ToList();
            var user2Phones = _context.UserPhones.Where(p => p.UserId == 2L).ToList();
            Assert.Single(user1Phones);
            Assert.Single(user2Phones);
        }

        [Fact]
        public void Insert_WithEmptyPhone_ShouldStillInsert()
        {
            // Arrange
            _mockPhoneModel.SetupGet(m => m.UserId).Returns(1L);
            _mockPhoneModel.SetupGet(m => m.Phone).Returns(string.Empty);

            // Act
            var result = _repository.Insert(_mockPhoneModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_context.UserPhones);
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingPhones_ShouldReturnPhones()
        {
            // Arrange
            var phone1 = new UserPhone { UserId = 1L, Phone = "1111111111" };
            var phone2 = new UserPhone { UserId = 1L, Phone = "2222222222" };
            _context.UserPhones.AddRange(phone1, phone2);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var phones = result.ToList();
            Assert.Equal(2, phones.Count);
            _mockFactory.Verify(f => f.BuildUserPhoneModel(), Times.Exactly(2));
        }

        [Fact]
        public void ListByUser_WithNoPhones_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListByUser(999L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListByUser_ShouldOnlyReturnPhonesForSpecificUser()
        {
            // Arrange
            var phone1 = new UserPhone { UserId = 1L, Phone = "1111111111" };
            var phone2 = new UserPhone { UserId = 2L, Phone = "2222222222" };
            var phone3 = new UserPhone { UserId = 1L, Phone = "3333333333" };
            _context.UserPhones.AddRange(phone1, phone2, phone3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var phones = result.ToList();
            Assert.Equal(2, phones.Count);
            _mockPhoneModel.VerifySet(m => m.Phone = "1111111111", Times.Once);
            _mockPhoneModel.VerifySet(m => m.Phone = "3333333333", Times.Once);
            _mockPhoneModel.VerifySet(m => m.Phone = "2222222222", Times.Never);
        }

        [Fact]
        public void ListByUser_ShouldMapAllFields()
        {
            // Arrange
            var phone = new UserPhone
            {
                UserId = 1L,
                Phone = "1234567890"
            };
            _context.UserPhones.Add(phone);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var phones = result.ToList();
            Assert.Single(phones);
            _mockPhoneModel.VerifySet(m => m.PhoneId = phone.PhoneId, Times.Once);
            _mockPhoneModel.VerifySet(m => m.UserId = 1L, Times.Once);
            _mockPhoneModel.VerifySet(m => m.Phone = "1234567890", Times.Once);
        }

        [Fact]
        public void ListByUser_WithZeroUserId_ShouldReturnEmpty()
        {
            // Arrange
            var phone = new UserPhone { UserId = 1L, Phone = "1234567890" };
            _context.UserPhones.Add(phone);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(0L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region DeleteAllByUser Tests

        [Fact]
        public void DeleteAllByUser_WithExistingPhones_ShouldDeleteAll()
        {
            // Arrange
            var phone1 = new UserPhone { UserId = 1L, Phone = "1111111111" };
            var phone2 = new UserPhone { UserId = 1L, Phone = "2222222222" };
            _context.UserPhones.AddRange(phone1, phone2);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);

            // Assert
            var remainingPhones = _context.UserPhones.Where(p => p.UserId == 1L).ToList();
            Assert.Empty(remainingPhones);
        }

        [Fact]
        public void DeleteAllByUser_WithNoPhones_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _repository.DeleteAllByUser(999L));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteAllByUser_ShouldOnlyDeletePhonesForSpecificUser()
        {
            // Arrange
            var phone1 = new UserPhone { UserId = 1L, Phone = "1111111111" };
            var phone2 = new UserPhone { UserId = 2L, Phone = "2222222222" };
            var phone3 = new UserPhone { UserId = 1L, Phone = "3333333333" };
            _context.UserPhones.AddRange(phone1, phone2, phone3);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);

            // Assert
            var user1Phones = _context.UserPhones.Where(p => p.UserId == 1L).ToList();
            var user2Phones = _context.UserPhones.Where(p => p.UserId == 2L).ToList();
            Assert.Empty(user1Phones);
            Assert.Single(user2Phones);
            Assert.Equal("2222222222", user2Phones[0].Phone);
        }

        [Fact]
        public void DeleteAllByUser_WithMultipleDeletions_ShouldWork()
        {
            // Arrange
            var phone1 = new UserPhone { UserId = 1L, Phone = "1111111111" };
            var phone2 = new UserPhone { UserId = 2L, Phone = "2222222222" };
            _context.UserPhones.AddRange(phone1, phone2);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);
            _repository.DeleteAllByUser(2L);

            // Assert
            Assert.Empty(_context.UserPhones);
        }

        [Fact]
        public void DeleteAllByUser_WithZeroUserId_ShouldNotDeleteAnything()
        {
            // Arrange
            var phone = new UserPhone { UserId = 1L, Phone = "1234567890" };
            _context.UserPhones.Add(phone);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(0L);

            // Assert
            Assert.Single(_context.UserPhones);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAndListByUser_ShouldWorkTogether()
        {
            // Arrange
            _mockPhoneModel.SetupGet(m => m.UserId).Returns(1L);
            _mockPhoneModel.SetupGet(m => m.Phone).Returns("1234567890");

            // Act
            _repository.Insert(_mockPhoneModel.Object, _mockFactory.Object);
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void InsertListAndDelete_ShouldWorkTogether()
        {
            // Arrange
            var phone = new UserPhone { UserId = 1L, Phone = "1234567890" };
            _context.UserPhones.Add(phone);
            _context.SaveChanges();

            // Act
            var beforeDelete = _repository.ListByUser(1L, _mockFactory.Object);
            _repository.DeleteAllByUser(1L);
            var afterDelete = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            Assert.Single(beforeDelete);
            Assert.Empty(afterDelete);
        }

        [Fact]
        public void CompleteWorkflow_InsertMultipleListAndDelete()
        {
            // Arrange & Act - Insert
            var mock1 = new Mock<IUserPhoneModel>();
            mock1.SetupGet(m => m.UserId).Returns(1L);
            mock1.SetupGet(m => m.Phone).Returns("1111111111");

            var mock2 = new Mock<IUserPhoneModel>();
            mock2.SetupGet(m => m.UserId).Returns(1L);
            mock2.SetupGet(m => m.Phone).Returns("2222222222");

            _repository.Insert(mock1.Object, _mockFactory.Object);
            _repository.Insert(mock2.Object, _mockFactory.Object);

            // Assert - List
            var phones = _repository.ListByUser(1L, _mockFactory.Object).ToList();
            Assert.Equal(2, phones.Count);

            // Act - Delete
            _repository.DeleteAllByUser(1L);

            // Assert - Verify deletion
            var afterDelete = _repository.ListByUser(1L, _mockFactory.Object);
            Assert.Empty(afterDelete);
        }

        [Fact]
        public void MultipleUsersWorkflow_ShouldIsolateData()
        {
            // Arrange & Act - Insert for User 1
            var user1Phone1 = new Mock<IUserPhoneModel>();
            user1Phone1.SetupGet(m => m.UserId).Returns(1L);
            user1Phone1.SetupGet(m => m.Phone).Returns("1111111111");

            var user1Phone2 = new Mock<IUserPhoneModel>();
            user1Phone2.SetupGet(m => m.UserId).Returns(1L);
            user1Phone2.SetupGet(m => m.Phone).Returns("1111111112");

            // Insert for User 2
            var user2Phone = new Mock<IUserPhoneModel>();
            user2Phone.SetupGet(m => m.UserId).Returns(2L);
            user2Phone.SetupGet(m => m.Phone).Returns("2222222222");

            _repository.Insert(user1Phone1.Object, _mockFactory.Object);
            _repository.Insert(user1Phone2.Object, _mockFactory.Object);
            _repository.Insert(user2Phone.Object, _mockFactory.Object);

            // Assert - Both users have phones
            var user1Phones = _repository.ListByUser(1L, _mockFactory.Object).ToList();
            var user2Phones = _repository.ListByUser(2L, _mockFactory.Object).ToList();
            Assert.Equal(2, user1Phones.Count);
            Assert.Single(user2Phones);

            // Act - Delete User 1's phones
            _repository.DeleteAllByUser(1L);

            // Assert - User 1 has no phones, User 2 still has phones
            var user1AfterDelete = _repository.ListByUser(1L, _mockFactory.Object);
            var user2AfterDelete = _repository.ListByUser(2L, _mockFactory.Object);
            Assert.Empty(user1AfterDelete);
            Assert.Single(user2AfterDelete);
        }

        #endregion
    }
}
