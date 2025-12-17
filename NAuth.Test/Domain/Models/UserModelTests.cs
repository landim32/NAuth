using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using Xunit;

namespace NAuth.Test.Domain.Models
{
    public class UserModelTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository<IUserModel, IUserDomainFactory>> _mockUserRepository;
        private readonly Mock<IUserRoleRepository<IRoleModel, IRoleDomainFactory>> _mockUserRoleRepository;
        private readonly Mock<IUserDomainFactory> _mockUserFactory;
        private readonly Mock<IRoleDomainFactory> _mockRoleFactory;
        private readonly UserModel _userModel;

        public UserModelTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepository = new Mock<IUserRepository<IUserModel, IUserDomainFactory>>();
            _mockUserRoleRepository = new Mock<IUserRoleRepository<IRoleModel, IRoleDomainFactory>>();
            _mockUserFactory = new Mock<IUserDomainFactory>();
            _mockRoleFactory = new Mock<IRoleDomainFactory>();
            _userModel = new UserModel(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockUserRoleRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var model = new UserModel(_mockUnitOfWork.Object, _mockUserRepository.Object, _mockUserRoleRepository.Object);

            // Assert
            Assert.NotNull(model);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_ShouldGetAndSetValues()
        {
            // Arrange
            var userId = 1L;
            var hash = "test-hash";
            var slug = "test-user";
            var image = "profile.jpg";
            var name = "Test User";
            var email = "test@test.com";
            var idDocument = "12345678900";
            var pixKey = "pix@test.com";
            var birthDate = new DateTime(1990, 1, 1);
            var isAdmin = true;
            var createdAt = DateTime.Now;
            var updatedAt = DateTime.Now;
            var stripeId = "stripe_123";

            // Act
            _userModel.UserId = userId;
            _userModel.Hash = hash;
            _userModel.Slug = slug;
            _userModel.Image = image;
            _userModel.Name = name;
            _userModel.Email = email;
            _userModel.IdDocument = idDocument;
            _userModel.PixKey = pixKey;
            _userModel.BirthDate = birthDate;
            _userModel.IsAdmin = isAdmin;
            _userModel.CreatedAt = createdAt;
            _userModel.UpdatedAt = updatedAt;
            _userModel.StripeId = stripeId;

            // Assert
            Assert.Equal(userId, _userModel.UserId);
            Assert.Equal(hash, _userModel.Hash);
            Assert.Equal(slug, _userModel.Slug);
            Assert.Equal(image, _userModel.Image);
            Assert.Equal(name, _userModel.Name);
            Assert.Equal(email, _userModel.Email);
            Assert.Equal(idDocument, _userModel.IdDocument);
            Assert.Equal(pixKey, _userModel.PixKey);
            Assert.Equal(birthDate, _userModel.BirthDate);
            Assert.Equal(isAdmin, _userModel.IsAdmin);
            Assert.Equal(createdAt, _userModel.CreatedAt);
            Assert.Equal(updatedAt, _userModel.UpdatedAt);
            Assert.Equal(stripeId, _userModel.StripeId);
        }

        [Fact]
        public void BirthDate_WithNullValue_ShouldAllowNull()
        {
            // Arrange & Act
            _userModel.BirthDate = null;

            // Assert
            Assert.Null(_userModel.BirthDate);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithExistingId_ShouldReturnUser()
        {
            // Arrange
            var userId = 1L;
            var mockUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns(mockUser.Object);

            // Act
            var result = _userModel.GetById(userId, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetById(userId, _mockUserFactory.Object), Times.Once);
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var userId = 999L;
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act
            var result = _userModel.GetById(userId, _mockUserFactory.Object);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.GetById(userId, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region GetByEmail Tests

        [Fact]
        public void GetByEmail_WithExistingEmail_ShouldReturnUser()
        {
            // Arrange
            var email = "test@test.com";
            var mockUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns(mockUser.Object);

            // Act
            var result = _userModel.GetByEmail(email, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetByEmail(email, _mockUserFactory.Object), Times.Once);
        }

        [Fact]
        public void GetByEmail_WithNonExistentEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexistent@test.com";
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act
            var result = _userModel.GetByEmail(email, _mockUserFactory.Object);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.GetByEmail(email, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region GetBySlug Tests

        [Fact]
        public void GetBySlug_WithExistingSlug_ShouldReturnUser()
        {
            // Arrange
            var slug = "test-user";
            var mockUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.GetBySlug(slug, _mockUserFactory.Object))
                .Returns(mockUser.Object);

            // Act
            var result = _userModel.GetBySlug(slug, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetBySlug(slug, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region GetByStripeId Tests

        [Fact]
        public void GetByStripeId_WithExistingStripeId_ShouldReturnUser()
        {
            // Arrange
            var stripeId = "stripe_123";
            var mockUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.GetByStripeId(stripeId, _mockUserFactory.Object))
                .Returns(mockUser.Object);

            // Act
            var result = _userModel.GetByStripeId(stripeId, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetByStripeId(stripeId, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region GetByRecoveryHash Tests

        [Fact]
        public void GetByRecoveryHash_WithExistingHash_ShouldReturnUser()
        {
            // Arrange
            var recoveryHash = "recovery_hash_123";
            var mockUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.GetUserByRecoveryHash(recoveryHash, _mockUserFactory.Object))
                .Returns(mockUser.Object);

            // Act
            var result = _userModel.GetByRecoveryHash(recoveryHash, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetUserByRecoveryHash(recoveryHash, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_ShouldCallRepositoryInsert()
        {
            // Arrange
            var mockReturnUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.Insert(_userModel, _mockUserFactory.Object))
                .Returns(mockReturnUser.Object);

            // Act
            var result = _userModel.Insert(_mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnUser.Object, result);
            _mockUserRepository.Verify(r => r.Insert(_userModel, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var mockReturnUser = new Mock<IUserModel>();
            _mockUserRepository
                .Setup(r => r.Update(_userModel, _mockUserFactory.Object))
                .Returns(mockReturnUser.Object);

            // Act
            var result = _userModel.Update(_mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnUser.Object, result);
            _mockUserRepository.Verify(r => r.Update(_userModel, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region ListUsers Tests

        [Fact]
        public void ListUsers_WithTake_ShouldReturnUsers()
        {
            // Arrange
            var take = 10;
            var mockUsers = new List<IUserModel>
            {
                Mock.Of<IUserModel>(),
                Mock.Of<IUserModel>(),
                Mock.Of<IUserModel>()
            };
            _mockUserRepository
                .Setup(r => r.ListUsers(take, _mockUserFactory.Object))
                .Returns(mockUsers);

            // Act
            var result = _userModel.ListUsers(take, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockUserRepository.Verify(r => r.ListUsers(take, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region LoginWithEmail Tests

        [Fact]
        public void LoginWithEmail_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var email = "test@test.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);
            
            var mockUser = new Mock<IUserModel>();
            mockUser.SetupGet(u => u.UserId).Returns(1L);
            
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.GetHashedPassword(1L))
                .Returns(hashedPassword);

            // Act
            var result = _userModel.LoginWithEmail(email, password, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockUser.Object, result);
            _mockUserRepository.Verify(r => r.GetByEmail(email, _mockUserFactory.Object), Times.Once);
            _mockUserRepository.Verify(r => r.GetHashedPassword(1L), Times.Once);
        }

        [Fact]
        public void LoginWithEmail_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var email = "test@test.com";
            var password = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword", 12);
            
            var mockUser = new Mock<IUserModel>();
            mockUser.SetupGet(u => u.UserId).Returns(1L);
            
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.GetHashedPassword(1L))
                .Returns(hashedPassword);

            // Act
            var result = _userModel.LoginWithEmail(email, password, _mockUserFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoginWithEmail_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var email = "nonexistent@test.com";
            var password = "password123";
            
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => 
                _userModel.LoginWithEmail(email, password, _mockUserFactory.Object));
        }

        [Fact]
        public void LoginWithEmail_WithNoPasswordSet_ShouldReturnNull()
        {
            // Arrange
            var email = "test@test.com";
            var password = "password123";
            
            var mockUser = new Mock<IUserModel>();
            mockUser.SetupGet(u => u.UserId).Returns(1L);
            
            _mockUserRepository
                .Setup(r => r.GetByEmail(email, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.GetHashedPassword(1L))
                .Returns(string.Empty);

            // Act
            var result = _userModel.LoginWithEmail(email, password, _mockUserFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region HasPassword Tests

        [Fact]
        public void HasPassword_WithPasswordSet_ShouldReturnTrue()
        {
            // Arrange
            var userId = 1L;
            _mockUserRepository
                .Setup(r => r.HasPassword(userId, _mockUserFactory.Object))
                .Returns(true);

            // Act
            var result = _userModel.HasPassword(userId, _mockUserFactory.Object);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.HasPassword(userId, _mockUserFactory.Object), Times.Once);
        }

        [Fact]
        public void HasPassword_WithoutPasswordSet_ShouldReturnFalse()
        {
            // Arrange
            var userId = 1L;
            _mockUserRepository
                .Setup(r => r.HasPassword(userId, _mockUserFactory.Object))
                .Returns(false);

            // Act
            var result = _userModel.HasPassword(userId, _mockUserFactory.Object);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.HasPassword(userId, _mockUserFactory.Object), Times.Once);
        }

        #endregion

        #region ChangePassword Tests

        [Fact]
        public void ChangePassword_WithExistingUser_ShouldChangePassword()
        {
            // Arrange
            var userId = 1L;
            var newPassword = "newpassword123";
            var mockUser = new Mock<IUserModel>();
            
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.ChangePassword(userId, It.IsAny<string>()));

            // Act
            _userModel.ChangePassword(userId, newPassword, _mockUserFactory.Object);

            // Assert
            _mockUserRepository.Verify(r => r.GetById(userId, _mockUserFactory.Object), Times.Once);
            _mockUserRepository.Verify(r => r.ChangePassword(userId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ChangePassword_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var userId = 999L;
            var newPassword = "newpassword123";
            
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => 
                _userModel.ChangePassword(userId, newPassword, _mockUserFactory.Object));
        }

        #endregion

        #region ChangePasswordUsingHash Tests

        [Fact]
        public void ChangePasswordUsingHash_WithValidHash_ShouldChangePassword()
        {
            // Arrange
            var recoveryHash = "valid_hash";
            var newPassword = "newpassword123";
            var mockUser = new Mock<IUserModel>();
            mockUser.SetupGet(u => u.UserId).Returns(1L);
            
            _mockUserRepository
                .Setup(r => r.GetUserByRecoveryHash(recoveryHash, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.ChangePassword(1L, It.IsAny<string>()));

            // Act
            _userModel.ChangePasswordUsingHash(recoveryHash, newPassword, _mockUserFactory.Object);

            // Assert
            _mockUserRepository.Verify(r => r.GetUserByRecoveryHash(recoveryHash, _mockUserFactory.Object), Times.Once);
            _mockUserRepository.Verify(r => r.ChangePassword(1L, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ChangePasswordUsingHash_WithInvalidHash_ShouldThrowException()
        {
            // Arrange
            var recoveryHash = "invalid_hash";
            var newPassword = "newpassword123";
            
            _mockUserRepository
                .Setup(r => r.GetUserByRecoveryHash(recoveryHash, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => 
                _userModel.ChangePasswordUsingHash(recoveryHash, newPassword, _mockUserFactory.Object));
        }

        #endregion

        #region GenerateRecoveryHash Tests

        [Fact]
        public void GenerateRecoveryHash_WithExistingUser_ShouldReturnHash()
        {
            // Arrange
            var userId = 1L;
            var mockUser = new Mock<IUserModel>();
            
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns(mockUser.Object);
            
            _mockUserRepository
                .Setup(r => r.UpdateRecoveryHash(userId, It.IsAny<string>()));

            // Act
            var result = _userModel.GenerateRecoveryHash(userId, _mockUserFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockUserRepository.Verify(r => r.GetById(userId, _mockUserFactory.Object), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateRecoveryHash(userId, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GenerateRecoveryHash_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var userId = 999L;
            
            _mockUserRepository
                .Setup(r => r.GetById(userId, _mockUserFactory.Object))
                .Returns((IUserModel)null!);

            // Act & Assert
            Assert.Throws<UserNotFoundException>(() => 
                _userModel.GenerateRecoveryHash(userId, _mockUserFactory.Object));
        }

        #endregion

        #region ExistSlug Tests

        [Fact]
        public void ExistSlug_WithExistingSlug_ShouldReturnTrue()
        {
            // Arrange
            var userId = 0L;
            var slug = "existing-slug";
            _mockUserRepository
                .Setup(r => r.ExistSlug(userId, slug))
                .Returns(true);

            // Act
            var result = _userModel.ExistSlug(userId, slug);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.ExistSlug(userId, slug), Times.Once);
        }

        [Fact]
        public void ExistSlug_WithNonExistentSlug_ShouldReturnFalse()
        {
            // Arrange
            var userId = 0L;
            var slug = "nonexistent-slug";
            _mockUserRepository
                .Setup(r => r.ExistSlug(userId, slug))
                .Returns(false);

            // Act
            var result = _userModel.ExistSlug(userId, slug);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.ExistSlug(userId, slug), Times.Once);
        }

        #endregion

        #region Role Management Tests

        [Fact]
        public void ListRoles_ShouldReturnUserRoles()
        {
            // Arrange
            var userId = 1L;
            var mockRoles = new List<IRoleModel>
            {
                Mock.Of<IRoleModel>(),
                Mock.Of<IRoleModel>()
            };
            _mockUserRoleRepository
                .Setup(r => r.ListRolesByUser(userId, _mockRoleFactory.Object))
                .Returns(mockRoles);

            // Act
            var result = _userModel.ListRoles(userId, _mockRoleFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUserRoleRepository.Verify(r => r.ListRolesByUser(userId, _mockRoleFactory.Object), Times.Once);
        }

        [Fact]
        public void AddRole_ShouldCallRepositoryAddRole()
        {
            // Arrange
            var userId = 1L;
            var roleId = 2L;
            _mockUserRoleRepository.Setup(r => r.AddRoleToUser(userId, roleId));

            // Act
            _userModel.AddRole(userId, roleId);

            // Assert
            _mockUserRoleRepository.Verify(r => r.AddRoleToUser(userId, roleId), Times.Once);
        }

        [Fact]
        public void RemoveRole_ShouldCallRepositoryRemoveRole()
        {
            // Arrange
            var userId = 1L;
            var roleId = 2L;
            _mockUserRoleRepository.Setup(r => r.RemoveRoleFromUser(userId, roleId));

            // Act
            _userModel.RemoveRole(userId, roleId);

            // Assert
            _mockUserRoleRepository.Verify(r => r.RemoveRoleFromUser(userId, roleId), Times.Once);
        }

        [Fact]
        public void RemoveAllRoles_ShouldCallRepositoryRemoveAllRoles()
        {
            // Arrange
            var userId = 1L;
            _mockUserRoleRepository.Setup(r => r.RemoveAllRolesFromUser(userId));

            // Act
            _userModel.RemoveAllRoles(userId);

            // Assert
            _mockUserRoleRepository.Verify(r => r.RemoveAllRolesFromUser(userId), Times.Once);
        }

        [Fact]
        public void HasRole_WithExistingRole_ShouldReturnTrue()
        {
            // Arrange
            var userId = 1L;
            var roleId = 2L;
            _mockUserRoleRepository
                .Setup(r => r.UserHasRole(userId, roleId))
                .Returns(true);

            // Act
            var result = _userModel.HasRole(userId, roleId);

            // Assert
            Assert.True(result);
            _mockUserRoleRepository.Verify(r => r.UserHasRole(userId, roleId), Times.Once);
        }

        [Fact]
        public void HasRoleBySlug_WithExistingRole_ShouldReturnTrue()
        {
            // Arrange
            var userId = 1L;
            var roleSlug = "admin";
            _mockUserRoleRepository
                .Setup(r => r.UserHasRoleBySlug(userId, roleSlug))
                .Returns(true);

            // Act
            var result = _userModel.HasRoleBySlug(userId, roleSlug);

            // Assert
            Assert.True(result);
            _mockUserRoleRepository.Verify(r => r.UserHasRoleBySlug(userId, roleSlug), Times.Once);
        }

        #endregion

        #region UserNotFoundException Tests

        [Fact]
        public void UserNotFoundException_ShouldHaveCorrectMessage()
        {
            // Arrange & Act
            var exception = new UserNotFoundException();

            // Assert
            Assert.Equal(UserModel.UserNotFoundMessage, exception.Message);
        }

        #endregion
    }
}
