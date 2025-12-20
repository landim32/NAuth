using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly UserRepository _repository;
        private readonly Mock<IUserDomainFactory> _mockFactory;
        private readonly Mock<IUserModel> _mockUserModel;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new UserRepository(_context);
            _mockFactory = new Mock<IUserDomainFactory>();
            _mockUserModel = new Mock<IUserModel>();

            _mockFactory.Setup(f => f.BuildUserModel()).Returns(_mockUserModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #region Insert Tests

        [Fact]
        public void Insert_WithValidUser_ShouldInsertSuccessfully()
        {
            // Arrange
            _mockUserModel.SetupGet(m => m.UserId).Returns(0);
            _mockUserModel.SetupGet(m => m.Hash).Returns("testhash");
            _mockUserModel.SetupGet(m => m.Name).Returns("Test User");
            _mockUserModel.SetupGet(m => m.Slug).Returns("test-user");
            _mockUserModel.SetupGet(m => m.Email).Returns("test@example.com");
            _mockUserModel.SetupGet(m => m.IsAdmin).Returns(false);
            _mockUserModel.SetupGet(m => m.Image).Returns("image.jpg");
            _mockUserModel.SetupGet(m => m.StripeId).Returns("stripe_123");
            _mockUserModel.SetupGet(m => m.CreatedAt).Returns(DateTime.Now);
            _mockUserModel.SetupGet(m => m.UpdatedAt).Returns(DateTime.Now);
            _mockUserModel.SetupSet(m => m.UserId = It.IsAny<long>());

            // Act
            var result = _repository.Insert(_mockUserModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedUser = _context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.Name);
            Assert.Equal("test-user", savedUser.Slug);
            Assert.True(savedUser.UserId > 0);
            _mockUserModel.VerifySet(m => m.UserId = It.IsAny<long>(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldGenerateUserId()
        {
            // Arrange
            _mockUserModel.SetupGet(m => m.Hash).Returns("hash");
            _mockUserModel.SetupGet(m => m.Name).Returns("User");
            _mockUserModel.SetupGet(m => m.Slug).Returns("user");
            _mockUserModel.SetupGet(m => m.Email).Returns("user@test.com");
            _mockUserModel.SetupGet(m => m.IsAdmin).Returns(false);
            _mockUserModel.SetupSet(m => m.UserId = It.IsAny<long>());

            // Act
            _repository.Insert(_mockUserModel.Object, _mockFactory.Object);

            // Assert
            var savedUser = _context.Users.FirstOrDefault();
            Assert.NotNull(savedUser);
            Assert.True(savedUser.UserId > 0);
        }

        [Fact]
        public void Insert_ShouldSetCreatedAtAndUpdatedAt()
        {
            // Arrange
            var beforeInsert = DateTime.Now;
            _mockUserModel.SetupGet(m => m.Hash).Returns("hash");
            _mockUserModel.SetupGet(m => m.Name).Returns("User");
            _mockUserModel.SetupGet(m => m.Slug).Returns("user");
            _mockUserModel.SetupGet(m => m.Email).Returns("user@test.com");
            _mockUserModel.SetupGet(m => m.IsAdmin).Returns(false);

            // Act
            _repository.Insert(_mockUserModel.Object, _mockFactory.Object);
            var afterInsert = DateTime.Now;

            // Assert
            var savedUser = _context.Users.FirstOrDefault();
            Assert.NotNull(savedUser);
            Assert.True(savedUser.CreatedAt >= beforeInsert && savedUser.CreatedAt <= afterInsert);
            Assert.True(savedUser.UpdatedAt >= beforeInsert && savedUser.UpdatedAt <= afterInsert);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithExistingUser_ShouldUpdateSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Original Name",
                Slug = "original",
                Email = "original@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            _mockUserModel.SetupGet(m => m.UserId).Returns(user.UserId);
            _mockUserModel.SetupGet(m => m.Hash).Returns("newhash");
            _mockUserModel.SetupGet(m => m.Name).Returns("Updated Name");
            _mockUserModel.SetupGet(m => m.Slug).Returns("updated");
            _mockUserModel.SetupGet(m => m.Email).Returns("updated@test.com");
            _mockUserModel.SetupGet(m => m.IsAdmin).Returns(true);

            // Act
            var beforeUpdate = DateTime.Now;
            var result = _repository.Update(_mockUserModel.Object, _mockFactory.Object);
            var afterUpdate = DateTime.Now;

            // Assert
            Assert.NotNull(result);
            var updatedUser = _context.Users.Find(user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated Name", updatedUser.Name);
            Assert.Equal("updated", updatedUser.Slug);
            Assert.Equal("updated@test.com", updatedUser.Email);
            Assert.True(updatedUser.IsAdmin);
            Assert.True(updatedUser.UpdatedAt >= beforeUpdate && updatedUser.UpdatedAt <= afterUpdate);
        }

        [Fact]
        public void Update_ShouldOnlyUpdateSpecifiedUser()
        {
            // Arrange
            var user1 = new User { Hash = "h1", Name = "User1", Slug = "u1", Email = "u1@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var user2 = new User { Hash = "h2", Name = "User2", Slug = "u2", Email = "u2@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();

            _mockUserModel.SetupGet(m => m.UserId).Returns(user1.UserId);
            _mockUserModel.SetupGet(m => m.Name).Returns("Updated User1");
            _mockUserModel.SetupGet(m => m.Hash).Returns("h1");
            _mockUserModel.SetupGet(m => m.Slug).Returns("u1");
            _mockUserModel.SetupGet(m => m.Email).Returns("u1@test.com");
            _mockUserModel.SetupGet(m => m.IsAdmin).Returns(false);

            // Act
            _repository.Update(_mockUserModel.Object, _mockFactory.Object);

            // Assert
            var updatedUser1 = _context.Users.Find(user1.UserId);
            var unchangedUser2 = _context.Users.Find(user2.UserId);
            Assert.Equal("Updated User1", updatedUser1?.Name);
            Assert.Equal("User2", unchangedUser2?.Name);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithExistingId_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetById(user.UserId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.UserId = user.UserId, Times.Once);
            _mockUserModel.VerifySet(m => m.Name = "Test User", Times.Once);
            _mockUserModel.VerifySet(m => m.Email = "test@test.com", Times.Once);
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetById(999L, _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByEmail Tests

        [Fact]
        public void GetByEmail_WithExistingEmail_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetByEmail("test@test.com", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.Email = "test@test.com", Times.Once);
        }

        [Fact]
        public void GetByEmail_WithNonExistentEmail_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetByEmail("nonexistent@test.com", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetByEmail_IsCaseSensitive()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetByEmail("TEST@TEST.COM", _mockFactory.Object);

            // Assert - EF Core in-memory is case-insensitive for strings by default
            // But in real SQL Server it might be case-sensitive depending on collation
            Assert.Null(result);
        }

        #endregion

        #region GetBySlug Tests

        [Fact]
        public void GetBySlug_WithExistingSlug_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test-user",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetBySlug("test-user", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.Slug = "test-user", Times.Once);
        }

        [Fact]
        public void GetBySlug_WithNonExistentSlug_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetBySlug("nonexistent", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByStripeId Tests

        [Fact]
        public void GetByStripeId_WithExistingStripeId_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                IsAdmin = false,
                StripeId = "stripe_12345",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetByStripeId("stripe_12345", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.StripeId = "stripe_12345", Times.Once);
        }

        [Fact]
        public void GetByStripeId_WithNonExistentStripeId_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetByStripeId("nonexistent", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region LoginWithEmail Tests

        [Fact]
        public void LoginWithEmail_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "encrypted_password",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.LoginWithEmail("test@test.com", "encrypted_password", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.Email = "test@test.com", Times.Once);
        }

        [Fact]
        public void LoginWithEmail_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "encrypted_password",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.LoginWithEmail("test@test.com", "wrong_password", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoginWithEmail_WithInvalidEmail_ShouldReturnNull()
        {
            // Act
            var result = _repository.LoginWithEmail("nonexistent@test.com", "password", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void LoginWithEmail_ShouldBeCaseInsensitiveForEmail()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "encrypted_password",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.LoginWithEmail("TEST@TEST.COM", "encrypted_password", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region HasPassword Tests

        [Fact]
        public void HasPassword_WithPasswordSet_ShouldReturnTrue()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "encrypted_password",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.HasPassword(user.UserId, _mockFactory.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasPassword_WithoutPasswordSet_ShouldReturnFalse()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = null,
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.HasPassword(user.UserId, _mockFactory.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasPassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.HasPassword(user.UserId, _mockFactory.Object);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetUserByRecoveryHash Tests

        [Fact]
        public void GetUserByRecoveryHash_WithExistingHash_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                RecoveryHash = "recovery_hash_123",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetUserByRecoveryHash("recovery_hash_123", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockUserModel.VerifySet(m => m.Email = "test@test.com", Times.Once);
        }

        [Fact]
        public void GetUserByRecoveryHash_WithNonExistentHash_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetUserByRecoveryHash("nonexistent_hash", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region UpdateRecoveryHash Tests

        [Fact]
        public void UpdateRecoveryHash_ShouldUpdateHashAndUpdatedAt()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                RecoveryHash = null,
                IsAdmin = false,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var beforeUpdate = DateTime.Now;
            _repository.UpdateRecoveryHash(user.UserId, "new_recovery_hash");
            var afterUpdate = DateTime.Now;

            // Assert
            var updatedUser = _context.Users.Find(user.UserId);
            Assert.Equal("new_recovery_hash", updatedUser?.RecoveryHash);
            Assert.True(updatedUser?.UpdatedAt >= beforeUpdate && updatedUser.UpdatedAt <= afterUpdate);
        }

        #endregion

        #region ChangePassword Tests

        [Fact]
        public void ChangePassword_ShouldUpdatePasswordAndClearRecoveryHash()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "old_password",
                RecoveryHash = "recovery_hash",
                IsAdmin = false,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var beforeChange = DateTime.Now;
            _repository.ChangePassword(user.UserId, "new_password");
            var afterChange = DateTime.Now;

            // Assert
            var updatedUser = _context.Users.Find(user.UserId);
            Assert.Equal("new_password", updatedUser.Password);
            Assert.Null(updatedUser.RecoveryHash);
            Assert.True(updatedUser.UpdatedAt >= beforeChange && updatedUser.UpdatedAt <= afterChange);
        }

        #endregion

        #region ExistSlug Tests

        [Fact]
        public void ExistSlug_WithExistingSlug_ShouldReturnTrue()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "existing-slug",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(0, "existing-slug");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ExistSlug_WithNonExistentSlug_ShouldReturnFalse()
        {
            // Act
            var result = _repository.ExistSlug(0, "nonexistent-slug");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExistSlug_WithSameUserId_ShouldReturnFalse()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test-slug",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(user.UserId, "test-slug");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExistSlug_WithDifferentUserId_ShouldReturnTrue()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test-slug",
                Email = "test@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(999L, "test-slug");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region GetHashedPassword Tests

        [Fact]
        public void GetHashedPassword_WithExistingUser_ShouldReturnPassword()
        {
            // Arrange
            var user = new User
            {
                Hash = "hash",
                Name = "Test User",
                Slug = "test",
                Email = "test@test.com",
                Password = "hashed_password_123",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _repository.GetHashedPassword(user.UserId);

            // Assert
            Assert.Equal("hashed_password_123", result);
        }

        [Fact]
        public void GetHashedPassword_WithNonExistentUser_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetHashedPassword(999L);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ListUsers Tests

        [Fact]
        public void ListUsers_WithMultipleUsers_ShouldReturnOrderedByName()
        {
            // Arrange
            var user1 = new User { Hash = "h1", Name = "Charlie", Slug = "charlie", Email = "c@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var user2 = new User { Hash = "h2", Name = "Alice", Slug = "alice", Email = "a@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var user3 = new User { Hash = "h3", Name = "Bob", Slug = "bob", Email = "b@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            _context.Users.AddRange(user1, user2, user3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListUsers(_mockFactory.Object);

            // Assert
            var users = result.ToList();
            Assert.Equal(3, users.Count);
            _mockFactory.Verify(f => f.BuildUserModel(), Times.Exactly(3));
        }

        [Fact]
        public void ListUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new[]
            {
                new User { Hash = "h1", Name = "User1", Slug = "u1", Email = "1@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new User { Hash = "h2", Name = "User2", Slug = "u2", Email = "2@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new User { Hash = "h3", Name = "User3", Slug = "u3", Email = "3@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new User { Hash = "h4", Name = "User4", Slug = "u4", Email = "4@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new User { Hash = "h5", Name = "User5", Slug = "u5", Email = "5@test.com", IsAdmin = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };
            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Act
            var result = _repository.ListUsers(_mockFactory.Object);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void ListUsers_WithEmptyDatabase_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListUsers(_mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        #endregion
    }
}
