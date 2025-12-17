using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class UserAddressRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly UserAddressRepository _repository;
        private readonly Mock<IUserAddressDomainFactory> _mockFactory;
        private readonly Mock<IUserAddressModel> _mockAddressModel;

        public UserAddressRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new UserAddressRepository(_context);
            _mockFactory = new Mock<IUserAddressDomainFactory>();
            _mockAddressModel = new Mock<IUserAddressModel>();

            _mockFactory.Setup(f => f.BuildUserAddressModel()).Returns(_mockAddressModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #region Insert Tests

        [Fact]
        public void Insert_WithValidAddress_ShouldInsertSuccessfully()
        {
            // Arrange
            _mockAddressModel.SetupGet(m => m.AddressId).Returns(0);
            _mockAddressModel.SetupGet(m => m.UserId).Returns(1L);
            _mockAddressModel.SetupGet(m => m.ZipCode).Returns("12345678");
            _mockAddressModel.SetupGet(m => m.Address).Returns("123 Test Street");
            _mockAddressModel.SetupGet(m => m.Complement).Returns("Apt 101");
            _mockAddressModel.SetupGet(m => m.Neighborhood).Returns("Test Neighborhood");
            _mockAddressModel.SetupGet(m => m.City).Returns("Test City");
            _mockAddressModel.SetupGet(m => m.State).Returns("TS");
            _mockAddressModel.SetupSet(m => m.AddressId = It.IsAny<long>());

            // Act
            var result = _repository.Insert(_mockAddressModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedAddress = _context.UserAddresses.FirstOrDefault(a => a.ZipCode == "12345678");
            Assert.NotNull(savedAddress);
            Assert.Equal(1L, savedAddress.UserId);
            Assert.Equal("123 Test Street", savedAddress.Address);
            Assert.Equal("Apt 101", savedAddress.Complement);
            Assert.Equal("Test Neighborhood", savedAddress.Neighborhood);
            Assert.Equal("Test City", savedAddress.City);
            Assert.Equal("TS", savedAddress.State);
            _mockAddressModel.VerifySet(m => m.AddressId = It.IsAny<long>(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldGenerateAddressId()
        {
            // Arrange
            _mockAddressModel.SetupGet(m => m.AddressId).Returns(0);
            _mockAddressModel.SetupGet(m => m.UserId).Returns(1L);
            _mockAddressModel.SetupGet(m => m.ZipCode).Returns("12345678");
            _mockAddressModel.SetupGet(m => m.Address).Returns("Test Street");
            _mockAddressModel.SetupGet(m => m.Complement).Returns("Apt 1");
            _mockAddressModel.SetupGet(m => m.Neighborhood).Returns("Test");
            _mockAddressModel.SetupGet(m => m.City).Returns("City");
            _mockAddressModel.SetupGet(m => m.State).Returns("ST");
            _mockAddressModel.SetupSet(m => m.AddressId = It.IsAny<long>());

            // Act
            _repository.Insert(_mockAddressModel.Object, _mockFactory.Object);

            // Assert
            var savedAddress = _context.UserAddresses.FirstOrDefault(a => a.ZipCode == "12345678");
            Assert.NotNull(savedAddress);
            Assert.True(savedAddress.AddressId > 0);
        }

        [Fact]
        public void Insert_MultipleAddresses_ShouldInsertAll()
        {
            // Arrange
            var mockAddress1 = new Mock<IUserAddressModel>();
            mockAddress1.SetupGet(m => m.UserId).Returns(1L);
            mockAddress1.SetupGet(m => m.ZipCode).Returns("11111111");
            mockAddress1.SetupGet(m => m.Address).Returns("Address 1");
            mockAddress1.SetupGet(m => m.Complement).Returns("Comp 1");
            mockAddress1.SetupGet(m => m.Neighborhood).Returns("Neigh 1");
            mockAddress1.SetupGet(m => m.City).Returns("City 1");
            mockAddress1.SetupGet(m => m.State).Returns("S1");

            var mockAddress2 = new Mock<IUserAddressModel>();
            mockAddress2.SetupGet(m => m.UserId).Returns(1L);
            mockAddress2.SetupGet(m => m.ZipCode).Returns("22222222");
            mockAddress2.SetupGet(m => m.Address).Returns("Address 2");
            mockAddress2.SetupGet(m => m.Complement).Returns("Comp 2");
            mockAddress2.SetupGet(m => m.Neighborhood).Returns("Neigh 2");
            mockAddress2.SetupGet(m => m.City).Returns("City 2");
            mockAddress2.SetupGet(m => m.State).Returns("S2");

            // Act
            _repository.Insert(mockAddress1.Object, _mockFactory.Object);
            _repository.Insert(mockAddress2.Object, _mockFactory.Object);

            // Assert
            Assert.Equal(2, _context.UserAddresses.Count());
        }

        [Fact]
        public void Insert_MultipleAddressesForDifferentUsers_ShouldInsertAll()
        {
            // Arrange
            var mockAddress1 = new Mock<IUserAddressModel>();
            mockAddress1.SetupGet(m => m.UserId).Returns(1L);
            mockAddress1.SetupGet(m => m.ZipCode).Returns("11111111");
            mockAddress1.SetupGet(m => m.Address).Returns("Address 1");
            mockAddress1.SetupGet(m => m.Complement).Returns("Comp 1");
            mockAddress1.SetupGet(m => m.Neighborhood).Returns("Neigh 1");
            mockAddress1.SetupGet(m => m.City).Returns("City 1");
            mockAddress1.SetupGet(m => m.State).Returns("S1");

            var mockAddress2 = new Mock<IUserAddressModel>();
            mockAddress2.SetupGet(m => m.UserId).Returns(2L);
            mockAddress2.SetupGet(m => m.ZipCode).Returns("22222222");
            mockAddress2.SetupGet(m => m.Address).Returns("Address 2");
            mockAddress2.SetupGet(m => m.Complement).Returns("Comp 2");
            mockAddress2.SetupGet(m => m.Neighborhood).Returns("Neigh 2");
            mockAddress2.SetupGet(m => m.City).Returns("City 2");
            mockAddress2.SetupGet(m => m.State).Returns("S2");

            // Act
            _repository.Insert(mockAddress1.Object, _mockFactory.Object);
            _repository.Insert(mockAddress2.Object, _mockFactory.Object);

            // Assert
            var user1Addresses = _context.UserAddresses.Where(a => a.UserId == 1L).ToList();
            var user2Addresses = _context.UserAddresses.Where(a => a.UserId == 2L).ToList();
            Assert.Single(user1Addresses);
            Assert.Single(user2Addresses);
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingAddresses_ShouldReturnAddresses()
        {
            // Arrange
            var address1 = new UserAddress
            {
                UserId = 1L,
                ZipCode = "11111111",
                Address = "Address 1",
                Complement = "Comp 1",
                Neighborhood = "Neigh 1",
                City = "City 1",
                State = "S1"
            };
            var address2 = new UserAddress
            {
                UserId = 1L,
                ZipCode = "22222222",
                Address = "Address 2",
                Complement = "Comp 2",
                Neighborhood = "Neigh 2",
                City = "City 2",
                State = "S2"
            };
            _context.UserAddresses.AddRange(address1, address2);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var addresses = result.ToList();
            Assert.Equal(2, addresses.Count);
            _mockFactory.Verify(f => f.BuildUserAddressModel(), Times.Exactly(2));
        }

        [Fact]
        public void ListByUser_WithNoAddresses_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListByUser(999L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListByUser_ShouldOnlyReturnAddressesForSpecificUser()
        {
            // Arrange
            var address1 = new UserAddress { UserId = 1L, ZipCode = "11111111", Address = "Addr1", Complement = "C1", Neighborhood = "N1", City = "C1", State = "S1" };
            var address2 = new UserAddress { UserId = 2L, ZipCode = "22222222", Address = "Addr2", Complement = "C2", Neighborhood = "N2", City = "C2", State = "S2" };
            var address3 = new UserAddress { UserId = 1L, ZipCode = "33333333", Address = "Addr3", Complement = "C3", Neighborhood = "N3", City = "C3", State = "S3" };
            _context.UserAddresses.AddRange(address1, address2, address3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var addresses = result.ToList();
            Assert.Equal(2, addresses.Count);
            _mockAddressModel.VerifySet(m => m.ZipCode = "11111111", Times.Once);
            _mockAddressModel.VerifySet(m => m.ZipCode = "33333333", Times.Once);
            _mockAddressModel.VerifySet(m => m.ZipCode = "22222222", Times.Never);
        }

        [Fact]
        public void ListByUser_ShouldMapAllFields()
        {
            // Arrange
            var address = new UserAddress
            {
                UserId = 1L,
                ZipCode = "12345678",
                Address = "123 Test Street",
                Complement = "Apt 101",
                Neighborhood = "Test Neighborhood",
                City = "Test City",
                State = "TS"
            };
            _context.UserAddresses.Add(address);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var addresses = result.ToList();
            Assert.Single(addresses);
            _mockAddressModel.VerifySet(m => m.AddressId = address.AddressId, Times.Once);
            _mockAddressModel.VerifySet(m => m.UserId = 1L, Times.Once);
            _mockAddressModel.VerifySet(m => m.ZipCode = "12345678", Times.Once);
            _mockAddressModel.VerifySet(m => m.Address = "123 Test Street", Times.Once);
            _mockAddressModel.VerifySet(m => m.Complement = "Apt 101", Times.Once);
            _mockAddressModel.VerifySet(m => m.Neighborhood = "Test Neighborhood", Times.Once);
            _mockAddressModel.VerifySet(m => m.City = "Test City", Times.Once);
            _mockAddressModel.VerifySet(m => m.State = "TS", Times.Once);
        }

        #endregion

        #region DeleteAllByUser Tests

        [Fact]
        public void DeleteAllByUser_WithExistingAddresses_ShouldDeleteAll()
        {
            // Arrange
            var address1 = new UserAddress { UserId = 1L, ZipCode = "11111111", Address = "A1", Complement = "C1", Neighborhood = "N1", City = "C1", State = "S1" };
            var address2 = new UserAddress { UserId = 1L, ZipCode = "22222222", Address = "A2", Complement = "C2", Neighborhood = "N2", City = "C2", State = "S2" };
            _context.UserAddresses.AddRange(address1, address2);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);

            // Assert
            var remainingAddresses = _context.UserAddresses.Where(a => a.UserId == 1L).ToList();
            Assert.Empty(remainingAddresses);
        }

        [Fact]
        public void DeleteAllByUser_WithNoAddresses_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _repository.DeleteAllByUser(999L));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteAllByUser_ShouldOnlyDeleteAddressesForSpecificUser()
        {
            // Arrange
            var address1 = new UserAddress { UserId = 1L, ZipCode = "11111111", Address = "A1", Complement = "C1", Neighborhood = "N1", City = "C1", State = "S1" };
            var address2 = new UserAddress { UserId = 2L, ZipCode = "22222222", Address = "A2", Complement = "C2", Neighborhood = "N2", City = "C2", State = "S2" };
            var address3 = new UserAddress { UserId = 1L, ZipCode = "33333333", Address = "A3", Complement = "C3", Neighborhood = "N3", City = "C3", State = "S3" };
            _context.UserAddresses.AddRange(address1, address2, address3);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);

            // Assert
            var user1Addresses = _context.UserAddresses.Where(a => a.UserId == 1L).ToList();
            var user2Addresses = _context.UserAddresses.Where(a => a.UserId == 2L).ToList();
            Assert.Empty(user1Addresses);
            Assert.Single(user2Addresses);
            Assert.Equal("22222222", user2Addresses[0].ZipCode);
        }

        [Fact]
        public void DeleteAllByUser_WithMultipleDeletions_ShouldWork()
        {
            // Arrange
            var address1 = new UserAddress { UserId = 1L, ZipCode = "11111111", Address = "A1", Complement = "C1", Neighborhood = "N1", City = "C1", State = "S1" };
            var address2 = new UserAddress { UserId = 2L, ZipCode = "22222222", Address = "A2", Complement = "C2", Neighborhood = "N2", City = "C2", State = "S2" };
            _context.UserAddresses.AddRange(address1, address2);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(1L);
            _repository.DeleteAllByUser(2L);

            // Assert
            Assert.Empty(_context.UserAddresses);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAndListByUser_ShouldWorkTogether()
        {
            // Arrange
            _mockAddressModel.SetupGet(m => m.UserId).Returns(1L);
            _mockAddressModel.SetupGet(m => m.ZipCode).Returns("12345678");
            _mockAddressModel.SetupGet(m => m.Address).Returns("Test Street");
            _mockAddressModel.SetupGet(m => m.Complement).Returns("Test Complement");
            _mockAddressModel.SetupGet(m => m.Neighborhood).Returns("Test Neighborhood");
            _mockAddressModel.SetupGet(m => m.City).Returns("Test City");
            _mockAddressModel.SetupGet(m => m.State).Returns("TS");

            // Act
            _repository.Insert(_mockAddressModel.Object, _mockFactory.Object);
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void InsertListAndDelete_ShouldWorkTogether()
        {
            // Arrange
            var mockAddress = new Mock<IUserAddressModel>();
            mockAddress.SetupGet(m => m.UserId).Returns(1L);
            mockAddress.SetupGet(m => m.ZipCode).Returns("12345678");
            mockAddress.SetupGet(m => m.Address).Returns("Test");
            mockAddress.SetupGet(m => m.Complement).Returns("Test");
            mockAddress.SetupGet(m => m.Neighborhood).Returns("Test");
            mockAddress.SetupGet(m => m.City).Returns("Test");
            mockAddress.SetupGet(m => m.State).Returns("TS");

            // Act
            _repository.Insert(mockAddress.Object, _mockFactory.Object);
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
            var mock1 = new Mock<IUserAddressModel>();
            mock1.SetupGet(m => m.UserId).Returns(1L);
            mock1.SetupGet(m => m.ZipCode).Returns("11111111");
            mock1.SetupGet(m => m.Address).Returns("Addr1");
            mock1.SetupGet(m => m.Complement).Returns("C1");
            mock1.SetupGet(m => m.Neighborhood).Returns("N1");
            mock1.SetupGet(m => m.City).Returns("City1");
            mock1.SetupGet(m => m.State).Returns("S1");

            var mock2 = new Mock<IUserAddressModel>();
            mock2.SetupGet(m => m.UserId).Returns(1L);
            mock2.SetupGet(m => m.ZipCode).Returns("22222222");
            mock2.SetupGet(m => m.Address).Returns("Addr2");
            mock2.SetupGet(m => m.Complement).Returns("C2");
            mock2.SetupGet(m => m.Neighborhood).Returns("N2");
            mock2.SetupGet(m => m.City).Returns("City2");
            mock2.SetupGet(m => m.State).Returns("S2");

            _repository.Insert(mock1.Object, _mockFactory.Object);
            _repository.Insert(mock2.Object, _mockFactory.Object);

            // Assert - List
            var addresses = _repository.ListByUser(1L, _mockFactory.Object).ToList();
            Assert.Equal(2, addresses.Count);

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
            var user1Address1 = new Mock<IUserAddressModel>();
            user1Address1.SetupGet(m => m.UserId).Returns(1L);
            user1Address1.SetupGet(m => m.ZipCode).Returns("11111111");
            user1Address1.SetupGet(m => m.Address).Returns("U1A1");
            user1Address1.SetupGet(m => m.Complement).Returns("C");
            user1Address1.SetupGet(m => m.Neighborhood).Returns("N");
            user1Address1.SetupGet(m => m.City).Returns("City");
            user1Address1.SetupGet(m => m.State).Returns("ST");

            var user1Address2 = new Mock<IUserAddressModel>();
            user1Address2.SetupGet(m => m.UserId).Returns(1L);
            user1Address2.SetupGet(m => m.ZipCode).Returns("11111112");
            user1Address2.SetupGet(m => m.Address).Returns("U1A2");
            user1Address2.SetupGet(m => m.Complement).Returns("C");
            user1Address2.SetupGet(m => m.Neighborhood).Returns("N");
            user1Address2.SetupGet(m => m.City).Returns("City");
            user1Address2.SetupGet(m => m.State).Returns("ST");

            // Insert for User 2
            var user2Address = new Mock<IUserAddressModel>();
            user2Address.SetupGet(m => m.UserId).Returns(2L);
            user2Address.SetupGet(m => m.ZipCode).Returns("22222222");
            user2Address.SetupGet(m => m.Address).Returns("U2A1");
            user2Address.SetupGet(m => m.Complement).Returns("C");
            user2Address.SetupGet(m => m.Neighborhood).Returns("N");
            user2Address.SetupGet(m => m.City).Returns("City");
            user2Address.SetupGet(m => m.State).Returns("ST");

            _repository.Insert(user1Address1.Object, _mockFactory.Object);
            _repository.Insert(user1Address2.Object, _mockFactory.Object);
            _repository.Insert(user2Address.Object, _mockFactory.Object);

            // Assert - Both users have addresses
            var user1Addresses = _repository.ListByUser(1L, _mockFactory.Object).ToList();
            var user2Addresses = _repository.ListByUser(2L, _mockFactory.Object).ToList();
            Assert.Equal(2, user1Addresses.Count);
            Assert.Single(user2Addresses);

            // Act - Delete User 1's addresses
            _repository.DeleteAllByUser(1L);

            // Assert - User 1 has no addresses, User 2 still has addresses
            var user1AfterDelete = _repository.ListByUser(1L, _mockFactory.Object);
            var user2AfterDelete = _repository.ListByUser(2L, _mockFactory.Object);
            Assert.Empty(user1AfterDelete);
            Assert.Single(user2AfterDelete);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Insert_WithEmptyFields_ShouldStillInsert()
        {
            // Arrange
            _mockAddressModel.SetupGet(m => m.UserId).Returns(1L);
            _mockAddressModel.SetupGet(m => m.ZipCode).Returns("");
            _mockAddressModel.SetupGet(m => m.Address).Returns("");
            _mockAddressModel.SetupGet(m => m.Complement).Returns("");
            _mockAddressModel.SetupGet(m => m.Neighborhood).Returns("");
            _mockAddressModel.SetupGet(m => m.City).Returns("");
            _mockAddressModel.SetupGet(m => m.State).Returns("");

            // Act
            var result = _repository.Insert(_mockAddressModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_context.UserAddresses);
        }

        [Fact]
        public void ListByUser_WithZeroUserId_ShouldReturnEmpty()
        {
            // Arrange
            var address = new UserAddress { UserId = 1L, ZipCode = "12345", Address = "Test", Complement = "C", Neighborhood = "N", City = "City", State = "ST" };
            _context.UserAddresses.Add(address);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(0L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void DeleteAllByUser_WithZeroUserId_ShouldNotDeleteAnything()
        {
            // Arrange
            var address = new UserAddress { UserId = 1L, ZipCode = "12345", Address = "Test", Complement = "C", Neighborhood = "N", City = "City", State = "ST" };
            _context.UserAddresses.Add(address);
            _context.SaveChanges();

            // Act
            _repository.DeleteAllByUser(0L);

            // Assert
            Assert.Single(_context.UserAddresses);
        }

        #endregion
    }
}
