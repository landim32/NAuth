using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using Xunit;

namespace NAuth.Test.Domain.Models
{
    public class UserAddressModelTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>> _mockRepository;
        private readonly Mock<IUserAddressDomainFactory> _mockFactory;
        private readonly UserAddressModel _addressModel;

        public UserAddressModelTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>>();
            _mockFactory = new Mock<IUserAddressDomainFactory>();
            _addressModel = new UserAddressModel(_mockUnitOfWork.Object, _mockRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var model = new UserAddressModel(_mockUnitOfWork.Object, _mockRepository.Object);

            // Assert
            Assert.NotNull(model);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_ShouldGetAndSetValues()
        {
            // Arrange
            var addressId = 1L;
            var userId = 100L;
            var zipCode = "12345678";
            var address = "123 Main Street";
            var complement = "Apt 101";
            var neighborhood = "Downtown";
            var city = "São Paulo";
            var state = "SP";

            // Act
            _addressModel.AddressId = addressId;
            _addressModel.UserId = userId;
            _addressModel.ZipCode = zipCode;
            _addressModel.Address = address;
            _addressModel.Complement = complement;
            _addressModel.Neighborhood = neighborhood;
            _addressModel.City = city;
            _addressModel.State = state;

            // Assert
            Assert.Equal(addressId, _addressModel.AddressId);
            Assert.Equal(userId, _addressModel.UserId);
            Assert.Equal(zipCode, _addressModel.ZipCode);
            Assert.Equal(address, _addressModel.Address);
            Assert.Equal(complement, _addressModel.Complement);
            Assert.Equal(neighborhood, _addressModel.Neighborhood);
            Assert.Equal(city, _addressModel.City);
            Assert.Equal(state, _addressModel.State);
        }

        [Fact]
        public void Properties_WithEmptyValues_ShouldAllowEmptyStrings()
        {
            // Arrange & Act
            _addressModel.ZipCode = "";
            _addressModel.Address = "";
            _addressModel.Complement = "";
            _addressModel.Neighborhood = "";
            _addressModel.City = "";
            _addressModel.State = "";

            // Assert
            Assert.Equal("", _addressModel.ZipCode);
            Assert.Equal("", _addressModel.Address);
            Assert.Equal("", _addressModel.Complement);
            Assert.Equal("", _addressModel.Neighborhood);
            Assert.Equal("", _addressModel.City);
            Assert.Equal("", _addressModel.State);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_ShouldCallRepositoryInsert()
        {
            // Arrange
            var mockReturnModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _addressModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Insert(_addressModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Insert_WithAddressData_ShouldPassCorrectData()
        {
            // Arrange
            _addressModel.UserId = 1L;
            _addressModel.ZipCode = "12345678";
            _addressModel.Address = "Test Street";
            _addressModel.Complement = "Apt 10";
            _addressModel.Neighborhood = "Center";
            _addressModel.City = "Test City";
            _addressModel.State = "TS";

            var mockReturnModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _addressModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(
                It.Is<IUserAddressModel>(m =>
                    m.UserId == 1L &&
                    m.ZipCode == "12345678" &&
                    m.Address == "Test Street" &&
                    m.Complement == "Apt 10" &&
                    m.Neighborhood == "Center" &&
                    m.City == "Test City" &&
                    m.State == "TS"),
                _mockFactory.Object),
                Times.Once);
        }

        [Fact]
        public void Insert_WithPartialData_ShouldStillInsert()
        {
            // Arrange
            _addressModel.UserId = 1L;
            _addressModel.ZipCode = "12345";
            _addressModel.Address = "Street";

            var mockReturnModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _addressModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(_addressModel, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingAddresses_ShouldReturnList()
        {
            // Arrange
            var userId = 1L;
            var mockAddresses = new List<IUserAddressModel>
            {
                Mock.Of<IUserAddressModel>(),
                Mock.Of<IUserAddressModel>(),
                Mock.Of<IUserAddressModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act
            var result = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var addresses = result.ToList();
            Assert.Equal(3, addresses.Count);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_WithNoAddresses_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = 999L;
            var mockAddresses = new List<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act
            var result = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_WithZeroUserId_ShouldCallRepository()
        {
            // Arrange
            var userId = 0L;
            var mockAddresses = new List<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act
            var result = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_ShouldReturnMaterializedList()
        {
            // Arrange
            var userId = 1L;
            var mockAddresses = new List<IUserAddressModel>
            {
                Mock.Of<IUserAddressModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act
            var result = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<List<IUserAddressModel>>(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region DeleteAllByUser Tests

        [Fact]
        public void DeleteAllByUser_ShouldCallRepositoryDelete()
        {
            // Arrange
            var userId = 1L;
            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act
            _addressModel.DeleteAllByUser(userId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Once);
        }

        [Fact]
        public void DeleteAllByUser_WithZeroUserId_ShouldStillCallRepository()
        {
            // Arrange
            var userId = 0L;
            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act
            _addressModel.DeleteAllByUser(userId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Once);
        }

        [Fact]
        public void DeleteAllByUser_WithNegativeUserId_ShouldStillCallRepository()
        {
            // Arrange
            var userId = -1L;
            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act
            _addressModel.DeleteAllByUser(userId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Once);
        }

        [Fact]
        public void DeleteAllByUser_MultipleCallsWithSameUserId_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var userId = 1L;
            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act
            _addressModel.DeleteAllByUser(userId);
            _addressModel.DeleteAllByUser(userId);
            _addressModel.DeleteAllByUser(userId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Exactly(3));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CompleteWorkflow_InsertAndList()
        {
            // Arrange
            var userId = 1L;
            _addressModel.UserId = userId;
            _addressModel.ZipCode = "12345678";
            _addressModel.Address = "Test Street";
            _addressModel.Complement = "Apt 10";
            _addressModel.Neighborhood = "Center";
            _addressModel.City = "Test City";
            _addressModel.State = "TS";

            var insertedModel = new Mock<IUserAddressModel>();
            insertedModel.SetupGet(m => m.AddressId).Returns(1L);
            insertedModel.SetupGet(m => m.UserId).Returns(userId);

            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(insertedModel.Object);

            var mockAddresses = new List<IUserAddressModel> { insertedModel.Object };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act - Insert
            var insertResult = _addressModel.Insert(_mockFactory.Object);

            // Act - List
            var listResult = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(listResult);
            Assert.Single(listResult);
            _mockRepository.Verify(r => r.Insert(_addressModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_InsertListAndDelete()
        {
            // Arrange
            var userId = 1L;
            _addressModel.UserId = userId;
            _addressModel.ZipCode = "12345678";
            _addressModel.Address = "Test Street";

            var insertedModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(insertedModel.Object);

            var mockAddresses = new List<IUserAddressModel> { insertedModel.Object };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act - Insert
            var insertResult = _addressModel.Insert(_mockFactory.Object);

            // Act - List
            var listResult = _addressModel.ListByUser(userId, _mockFactory.Object);

            // Act - Delete
            _addressModel.DeleteAllByUser(userId);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(listResult);
            Assert.Single(listResult);
            _mockRepository.Verify(r => r.Insert(_addressModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Once);
        }

        [Fact]
        public void MultipleAddresses_ForSameUser_ShouldWorkIndependently()
        {
            // Arrange
            var userId = 1L;

            var address1 = new UserAddressModel(_mockUnitOfWork.Object, _mockRepository.Object);
            address1.UserId = userId;
            address1.Address = "Address 1";

            var address2 = new UserAddressModel(_mockUnitOfWork.Object, _mockRepository.Object);
            address2.UserId = userId;
            address2.Address = "Address 2";

            var mockReturn1 = Mock.Of<IUserAddressModel>();
            var mockReturn2 = Mock.Of<IUserAddressModel>();

            _mockRepository
                .Setup(r => r.Insert(address1, _mockFactory.Object))
                .Returns(mockReturn1);

            _mockRepository
                .Setup(r => r.Insert(address2, _mockFactory.Object))
                .Returns(mockReturn2);

            // Act
            var result1 = address1.Insert(_mockFactory.Object);
            var result2 = address2.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            _mockRepository.Verify(r => r.Insert(address1, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Insert(address2, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ModelState_ShouldMaintainValuesBetweenOperations()
        {
            // Arrange
            var userId = 1L;
            _addressModel.UserId = userId;
            _addressModel.ZipCode = "12345678";
            _addressModel.Address = "Test Street";
            _addressModel.City = "Test City";

            var mockReturnModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            _addressModel.Insert(_mockFactory.Object);

            // Assert - Values should remain unchanged after operation
            Assert.Equal(userId, _addressModel.UserId);
            Assert.Equal("12345678", _addressModel.ZipCode);
            Assert.Equal("Test Street", _addressModel.Address);
            Assert.Equal("Test City", _addressModel.City);
        }

        [Fact]
        public void ListByUser_CalledMultipleTimes_ShouldInvokeRepositoryEachTime()
        {
            // Arrange
            var userId = 1L;
            var mockAddresses = new List<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockAddresses);

            // Act
            _addressModel.ListByUser(userId, _mockFactory.Object);
            _addressModel.ListByUser(userId, _mockFactory.Object);
            _addressModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Exactly(3));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Insert_WithNullValues_ShouldNotThrowException()
        {
            // Arrange
            _addressModel.UserId = 1L;
            _addressModel.ZipCode = null;
            _addressModel.Address = null;
            _addressModel.Complement = null;
            _addressModel.Neighborhood = null;
            _addressModel.City = null;
            _addressModel.State = null;

            var mockReturnModel = new Mock<IUserAddressModel>();
            _mockRepository
                .Setup(r => r.Insert(_addressModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _addressModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(_addressModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void AllProperties_WithLongStrings_ShouldHandleCorrectly()
        {
            // Arrange
            var longString = new string('A', 1000);

            // Act
            _addressModel.ZipCode = longString;
            _addressModel.Address = longString;
            _addressModel.Complement = longString;
            _addressModel.Neighborhood = longString;
            _addressModel.City = longString;
            _addressModel.State = longString;

            // Assert
            Assert.Equal(longString, _addressModel.ZipCode);
            Assert.Equal(longString, _addressModel.Address);
            Assert.Equal(longString, _addressModel.Complement);
            Assert.Equal(longString, _addressModel.Neighborhood);
            Assert.Equal(longString, _addressModel.City);
            Assert.Equal(longString, _addressModel.State);
        }

        [Fact]
        public void Properties_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            _addressModel.Address = specialChars;
            _addressModel.Complement = specialChars;
            _addressModel.Neighborhood = specialChars;
            _addressModel.City = specialChars;

            // Assert
            Assert.Equal(specialChars, _addressModel.Address);
            Assert.Equal(specialChars, _addressModel.Complement);
            Assert.Equal(specialChars, _addressModel.Neighborhood);
            Assert.Equal(specialChars, _addressModel.City);
        }

        #endregion
    }
}
