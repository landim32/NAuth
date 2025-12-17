using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using Xunit;

namespace NAuth.Test.Domain.Models
{
    public class UserPhoneModelTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>> _mockRepository;
        private readonly Mock<IUserPhoneDomainFactory> _mockFactory;
        private readonly UserPhoneModel _phoneModel;

        public UserPhoneModelTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>>();
            _mockFactory = new Mock<IUserPhoneDomainFactory>();
            _phoneModel = new UserPhoneModel(_mockUnitOfWork.Object, _mockRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var model = new UserPhoneModel(_mockUnitOfWork.Object, _mockRepository.Object);

            // Assert
            Assert.NotNull(model);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_ShouldGetAndSetValues()
        {
            // Arrange
            var phoneId = 1L;
            var userId = 100L;
            var phone = "11999887766";

            // Act
            _phoneModel.PhoneId = phoneId;
            _phoneModel.UserId = userId;
            _phoneModel.Phone = phone;

            // Assert
            Assert.Equal(phoneId, _phoneModel.PhoneId);
            Assert.Equal(userId, _phoneModel.UserId);
            Assert.Equal(phone, _phoneModel.Phone);
        }

        [Fact]
        public void Phone_WithEmptyString_ShouldAllowEmptyString()
        {
            // Arrange & Act
            _phoneModel.Phone = "";

            // Assert
            Assert.Equal("", _phoneModel.Phone);
        }

        [Fact]
        public void Phone_WithNull_ShouldAllowNull()
        {
            // Arrange & Act
            _phoneModel.Phone = null;

            // Assert
            Assert.Null(_phoneModel.Phone);
        }

        [Fact]
        public void Phone_WithDifferentFormats_ShouldStoreAsIs()
        {
            // Arrange
            var formats = new[]
            {
                "11999887766",
                "(11) 99988-7766",
                "+55 11 99988-7766",
                "11 9 9988-7766"
            };

            // Act & Assert
            foreach (var format in formats)
            {
                _phoneModel.Phone = format;
                Assert.Equal(format, _phoneModel.Phone);
            }
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_ShouldCallRepositoryInsert()
        {
            // Arrange
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _phoneModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Insert_WithPhoneData_ShouldPassCorrectData()
        {
            // Arrange
            _phoneModel.UserId = 1L;
            _phoneModel.Phone = "11999887766";
            
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _phoneModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(
                It.Is<IUserPhoneModel>(m => 
                    m.UserId == 1L &&
                    m.Phone == "11999887766"),
                _mockFactory.Object),
                Times.Once);
        }

        [Fact]
        public void Insert_WithPartialData_ShouldStillInsert()
        {
            // Arrange
            _phoneModel.UserId = 1L;
            _phoneModel.Phone = "11999";
            
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _phoneModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Insert_WithEmptyPhone_ShouldCallRepository()
        {
            // Arrange
            _phoneModel.UserId = 1L;
            _phoneModel.Phone = "";
            
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _phoneModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingPhones_ShouldReturnList()
        {
            // Arrange
            var userId = 1L;
            var mockPhones = new List<IUserPhoneModel>
            {
                Mock.Of<IUserPhoneModel>(),
                Mock.Of<IUserPhoneModel>(),
                Mock.Of<IUserPhoneModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act
            var result = _phoneModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var phones = result.ToList();
            Assert.Equal(3, phones.Count);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_WithNoPhones_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = 999L;
            var mockPhones = new List<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act
            var result = _phoneModel.ListByUser(userId, _mockFactory.Object);

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
            var mockPhones = new List<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act
            var result = _phoneModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_ShouldReturnMaterializedList()
        {
            // Arrange
            var userId = 1L;
            var mockPhones = new List<IUserPhoneModel>
            {
                Mock.Of<IUserPhoneModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act
            var result = _phoneModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<List<IUserPhoneModel>>(result);
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
            _phoneModel.DeleteAllByUser(userId);

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
            _phoneModel.DeleteAllByUser(userId);

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
            _phoneModel.DeleteAllByUser(userId);

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
            _phoneModel.DeleteAllByUser(userId);
            _phoneModel.DeleteAllByUser(userId);
            _phoneModel.DeleteAllByUser(userId);

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
            _phoneModel.UserId = userId;
            _phoneModel.Phone = "11999887766";
            
            var insertedModel = new Mock<IUserPhoneModel>();
            insertedModel.SetupGet(m => m.PhoneId).Returns(1L);
            insertedModel.SetupGet(m => m.UserId).Returns(userId);
            
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(insertedModel.Object);
            
            var mockPhones = new List<IUserPhoneModel> { insertedModel.Object };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act - Insert
            var insertResult = _phoneModel.Insert(_mockFactory.Object);
            
            // Act - List
            var listResult = _phoneModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(listResult);
            Assert.Single(listResult);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_InsertListAndDelete()
        {
            // Arrange
            var userId = 1L;
            _phoneModel.UserId = userId;
            _phoneModel.Phone = "11999887766";
            
            var insertedModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(insertedModel.Object);
            
            var mockPhones = new List<IUserPhoneModel> { insertedModel.Object };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);
            
            _mockRepository.Setup(r => r.DeleteAllByUser(userId));

            // Act - Insert
            var insertResult = _phoneModel.Insert(_mockFactory.Object);
            
            // Act - List
            var listResult = _phoneModel.ListByUser(userId, _mockFactory.Object);
            
            // Act - Delete
            _phoneModel.DeleteAllByUser(userId);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(listResult);
            Assert.Single(listResult);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.DeleteAllByUser(userId), Times.Once);
        }

        [Fact]
        public void MultiplePhones_ForSameUser_ShouldWorkIndependently()
        {
            // Arrange
            var userId = 1L;
            
            var phone1 = new UserPhoneModel(_mockUnitOfWork.Object, _mockRepository.Object);
            phone1.UserId = userId;
            phone1.Phone = "11999887766";
            
            var phone2 = new UserPhoneModel(_mockUnitOfWork.Object, _mockRepository.Object);
            phone2.UserId = userId;
            phone2.Phone = "11988776655";
            
            var mockReturn1 = Mock.Of<IUserPhoneModel>();
            var mockReturn2 = Mock.Of<IUserPhoneModel>();
            
            _mockRepository
                .Setup(r => r.Insert(phone1, _mockFactory.Object))
                .Returns(mockReturn1);
            
            _mockRepository
                .Setup(r => r.Insert(phone2, _mockFactory.Object))
                .Returns(mockReturn2);

            // Act
            var result1 = phone1.Insert(_mockFactory.Object);
            var result2 = phone2.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            _mockRepository.Verify(r => r.Insert(phone1, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Insert(phone2, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ModelState_ShouldMaintainValuesBetweenOperations()
        {
            // Arrange
            var userId = 1L;
            _phoneModel.UserId = userId;
            _phoneModel.Phone = "11999887766";
            
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            _phoneModel.Insert(_mockFactory.Object);

            // Assert - Values should remain unchanged after operation
            Assert.Equal(userId, _phoneModel.UserId);
            Assert.Equal("11999887766", _phoneModel.Phone);
        }

        [Fact]
        public void ListByUser_CalledMultipleTimes_ShouldInvokeRepositoryEachTime()
        {
            // Arrange
            var userId = 1L;
            var mockPhones = new List<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockPhones);

            // Act
            _phoneModel.ListByUser(userId, _mockFactory.Object);
            _phoneModel.ListByUser(userId, _mockFactory.Object);
            _phoneModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Exactly(3));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Insert_WithNullPhone_ShouldNotThrowException()
        {
            // Arrange
            _phoneModel.UserId = 1L;
            _phoneModel.Phone = null;
            
            var mockReturnModel = new Mock<IUserPhoneModel>();
            _mockRepository
                .Setup(r => r.Insert(_phoneModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _phoneModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(_phoneModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Phone_WithVeryLongString_ShouldHandleCorrectly()
        {
            // Arrange
            var longPhone = new string('9', 100);

            // Act
            _phoneModel.Phone = longPhone;

            // Assert
            Assert.Equal(longPhone, _phoneModel.Phone);
            Assert.Equal(100, _phoneModel.Phone.Length);
        }

        [Fact]
        public void Phone_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var specialChars = "+55 (11) 99988-7766 ext. 123";

            // Act
            _phoneModel.Phone = specialChars;

            // Assert
            Assert.Equal(specialChars, _phoneModel.Phone);
        }

        [Fact]
        public void Phone_WithInternationalFormat_ShouldStoreAsIs()
        {
            // Arrange
            var internationalFormats = new[]
            {
                "+55 11 99988-7766",
                "+1 (555) 123-4567",
                "+44 20 7946 0958",
                "+86 10 1234 5678"
            };

            // Act & Assert
            foreach (var format in internationalFormats)
            {
                _phoneModel.Phone = format;
                Assert.Equal(format, _phoneModel.Phone);
            }
        }

        [Fact]
        public void Properties_WithZeroValues_ShouldStoreCorrectly()
        {
            // Arrange & Act
            _phoneModel.PhoneId = 0L;
            _phoneModel.UserId = 0L;
            _phoneModel.Phone = "0";

            // Assert
            Assert.Equal(0L, _phoneModel.PhoneId);
            Assert.Equal(0L, _phoneModel.UserId);
            Assert.Equal("0", _phoneModel.Phone);
        }

        #endregion
    }
}
