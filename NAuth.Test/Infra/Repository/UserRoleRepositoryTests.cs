using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class UserRoleRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly UserRoleRepository _repository;
        private readonly Mock<IRoleDomainFactory> _mockFactory;
        private readonly Mock<IRoleModel> _mockRoleModel;

        public UserRoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new UserRoleRepository(_context);
            _mockFactory = new Mock<IRoleDomainFactory>();
            _mockRoleModel = new Mock<IRoleModel>();

            _mockFactory.Setup(f => f.BuildRoleModel()).Returns(_mockRoleModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region AddRoleToUser Tests

        [Fact]
        public void AddRoleToUser_WithValidUserAndRole_ShouldAddRole()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Single(updatedUser.Roles);
            Assert.Contains(updatedUser.Roles, r => r.RoleId == role.RoleId);
        }

        [Fact]
        public void AddRoleToUser_WithNonExistentUser_ShouldNotThrowException()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act & Assert
            var exception = Record.Exception(() => _repository.AddRoleToUser(999L, role.RoleId));
            Assert.Null(exception);
        }

        [Fact]
        public void AddRoleToUser_WithNonExistentRole_ShouldNotThrowException()
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

            // Act & Assert
            var exception = Record.Exception(() => _repository.AddRoleToUser(user.UserId, 999L));
            Assert.Null(exception);
        }

        [Fact]
        public void AddRoleToUser_WithDuplicateRole_ShouldNotAddAgain()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            _repository.AddRoleToUser(user.UserId, role.RoleId);
            _repository.AddRoleToUser(user.UserId, role.RoleId); // Add again

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Single(updatedUser.Roles); // Should still have only one role
        }

        [Fact]
        public void AddRoleToUser_MultipleRoles_ShouldAddAll()
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
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Users.Add(user);
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();

            // Act
            _repository.AddRoleToUser(user.UserId, role1.RoleId);
            _repository.AddRoleToUser(user.UserId, role2.RoleId);
            _repository.AddRoleToUser(user.UserId, role3.RoleId);

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Equal(3, updatedUser.Roles.Count);
        }

        #endregion

        #region RemoveRoleFromUser Tests

        [Fact]
        public void RemoveRoleFromUser_WithExistingRole_ShouldRemoveRole()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Act
            _repository.RemoveRoleFromUser(user.UserId, role.RoleId);

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Empty(updatedUser.Roles);
        }

        [Fact]
        public void RemoveRoleFromUser_WithNonExistentUser_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _repository.RemoveRoleFromUser(999L, 1L));
            Assert.Null(exception);
        }

        [Fact]
        public void RemoveRoleFromUser_WithNonExistentRole_ShouldNotThrowException()
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

            // Act & Assert
            var exception = Record.Exception(() => _repository.RemoveRoleFromUser(user.UserId, 999L));
            Assert.Null(exception);
        }

        [Fact]
        public void RemoveRoleFromUser_ShouldOnlyRemoveSpecifiedRole()
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
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            _context.Users.Add(user);
            _context.Roles.AddRange(role1, role2);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role1.RoleId);
            _repository.AddRoleToUser(user.UserId, role2.RoleId);

            // Act
            _repository.RemoveRoleFromUser(user.UserId, role1.RoleId);

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Single(updatedUser.Roles);
            Assert.Contains(updatedUser.Roles, r => r.RoleId == role2.RoleId);
            Assert.DoesNotContain(updatedUser.Roles, r => r.RoleId == role1.RoleId);
        }

        #endregion

        #region RemoveAllRolesFromUser Tests

        [Fact]
        public void RemoveAllRolesFromUser_WithMultipleRoles_ShouldRemoveAll()
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
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Users.Add(user);
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role1.RoleId);
            _repository.AddRoleToUser(user.UserId, role2.RoleId);
            _repository.AddRoleToUser(user.UserId, role3.RoleId);

            // Act
            _repository.RemoveAllRolesFromUser(user.UserId);

            // Assert
            var updatedUser = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user.UserId);
            Assert.NotNull(updatedUser);
            Assert.Empty(updatedUser.Roles);
        }

        [Fact]
        public void RemoveAllRolesFromUser_WithNoRoles_ShouldNotThrowException()
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

            // Act & Assert
            var exception = Record.Exception(() => _repository.RemoveAllRolesFromUser(user.UserId));
            Assert.Null(exception);
        }

        [Fact]
        public void RemoveAllRolesFromUser_WithNonExistentUser_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _repository.RemoveAllRolesFromUser(999L));
            Assert.Null(exception);
        }

        [Fact]
        public void RemoveAllRolesFromUser_ShouldOnlyAffectSpecifiedUser()
        {
            // Arrange
            var user1 = new User
            {
                Hash = "hash1",
                Name = "User 1",
                Slug = "user1",
                Email = "user1@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var user2 = new User
            {
                Hash = "hash2",
                Name = "User 2",
                Slug = "user2",
                Email = "user2@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.AddRange(user1, user2);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user1.UserId, role.RoleId);
            _repository.AddRoleToUser(user2.UserId, role.RoleId);

            // Act
            _repository.RemoveAllRolesFromUser(user1.UserId);

            // Assert
            var updatedUser1 = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user1.UserId);
            var updatedUser2 = _context.Users.Include(u => u.Roles).FirstOrDefault(u => u.UserId == user2.UserId);
            Assert.NotNull(updatedUser1);
            Assert.NotNull(updatedUser2);
            Assert.Empty(updatedUser1.Roles);
            Assert.Single(updatedUser2.Roles);
        }

        #endregion

        #region UserHasRole Tests

        [Fact]
        public void UserHasRole_WithExistingRole_ShouldReturnTrue()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Act
            var result = _repository.UserHasRole(user.UserId, role.RoleId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UserHasRole_WithNonExistentRole_ShouldReturnFalse()
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
            var result = _repository.UserHasRole(user.UserId, 999L);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasRole_WithNonExistentUser_ShouldReturnFalse()
        {
            // Act
            var result = _repository.UserHasRole(999L, 1L);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasRole_AfterRemovingRole_ShouldReturnFalse()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Act
            _repository.RemoveRoleFromUser(user.UserId, role.RoleId);
            var result = _repository.UserHasRole(user.UserId, role.RoleId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region UserHasRoleBySlug Tests

        [Fact]
        public void UserHasRoleBySlug_WithExistingRole_ShouldReturnTrue()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Act
            var result = _repository.UserHasRoleBySlug(user.UserId, "admin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UserHasRoleBySlug_WithNonExistentRole_ShouldReturnFalse()
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
            var result = _repository.UserHasRoleBySlug(user.UserId, "nonexistent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasRoleBySlug_WithNonExistentUser_ShouldReturnFalse()
        {
            // Act
            var result = _repository.UserHasRoleBySlug(999L, "admin");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UserHasRoleBySlug_IsCaseSensitive()
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
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.Add(user);
            _context.Roles.Add(role);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role.RoleId);

            // Act
            var result = _repository.UserHasRoleBySlug(user.UserId, "ADMIN");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ListRolesByUser Tests

        [Fact]
        public void ListRolesByUser_WithMultipleRoles_ShouldReturnOrderedByName()
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
            var role1 = new Role { Slug = "user", Name = "User" };
            var role2 = new Role { Slug = "admin", Name = "Administrator" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Users.Add(user);
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();
            _repository.AddRoleToUser(user.UserId, role1.RoleId);
            _repository.AddRoleToUser(user.UserId, role2.RoleId);
            _repository.AddRoleToUser(user.UserId, role3.RoleId);

            // Act
            var result = _repository.ListRolesByUser(user.UserId, _mockFactory.Object);

            // Assert
            var roles = result.ToList();
            Assert.Equal(3, roles.Count);
            _mockFactory.Verify(f => f.BuildRoleModel(), Times.Exactly(3));
        }

        [Fact]
        public void ListRolesByUser_WithNoRoles_ShouldReturnEmpty()
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
            var result = _repository.ListRolesByUser(user.UserId, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListRolesByUser_WithNonExistentUser_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListRolesByUser(999L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListRolesByUser_ShouldOnlyReturnRolesForSpecificUser()
        {
            // Arrange
            var user1 = new User
            {
                Hash = "hash1",
                Name = "User 1",
                Slug = "user1",
                Email = "user1@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var user2 = new User
            {
                Hash = "hash2",
                Name = "User 2",
                Slug = "user2",
                Email = "user2@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            _context.Users.AddRange(user1, user2);
            _context.Roles.AddRange(role1, role2);
            _context.SaveChanges();
            _repository.AddRoleToUser(user1.UserId, role1.RoleId);
            _repository.AddRoleToUser(user2.UserId, role2.RoleId);

            // Act
            var result = _repository.ListRolesByUser(user1.UserId, _mockFactory.Object);

            // Assert
            var roles = result.ToList();
            Assert.Single(roles);
            _mockRoleModel.VerifySet(m => m.Slug = "admin", Times.Once);
            _mockRoleModel.VerifySet(m => m.Slug = "user", Times.Never);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CompleteWorkflow_AddListCheckAndRemove()
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
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            _context.Users.Add(user);
            _context.Roles.AddRange(role1, role2);
            _context.SaveChanges();

            // Act - Add roles
            _repository.AddRoleToUser(user.UserId, role1.RoleId);
            _repository.AddRoleToUser(user.UserId, role2.RoleId);

            // Assert - List roles
            var roles = _repository.ListRolesByUser(user.UserId, _mockFactory.Object).ToList();
            Assert.Equal(2, roles.Count);

            // Assert - Check roles
            Assert.True(_repository.UserHasRole(user.UserId, role1.RoleId));
            Assert.True(_repository.UserHasRole(user.UserId, role2.RoleId));
            Assert.True(_repository.UserHasRoleBySlug(user.UserId, "admin"));
            Assert.True(_repository.UserHasRoleBySlug(user.UserId, "user"));

            // Act - Remove one role
            _repository.RemoveRoleFromUser(user.UserId, role1.RoleId);

            // Assert - Check after removal
            Assert.False(_repository.UserHasRole(user.UserId, role1.RoleId));
            Assert.True(_repository.UserHasRole(user.UserId, role2.RoleId));

            // Act - Remove all roles
            _repository.RemoveAllRolesFromUser(user.UserId);

            // Assert - Check after removing all
            var rolesAfterRemoveAll = _repository.ListRolesByUser(user.UserId, _mockFactory.Object);
            Assert.Empty(rolesAfterRemoveAll);
        }

        [Fact]
        public void MultipleUsersWorkflow_ShouldIsolateData()
        {
            // Arrange
            var user1 = new User
            {
                Hash = "hash1",
                Name = "User 1",
                Slug = "user1",
                Email = "user1@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var user2 = new User
            {
                Hash = "hash2",
                Name = "User 2",
                Slug = "user2",
                Email = "user2@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Users.AddRange(user1, user2);
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();

            // Act - Add roles to users
            _repository.AddRoleToUser(user1.UserId, role1.RoleId);
            _repository.AddRoleToUser(user1.UserId, role2.RoleId);
            _repository.AddRoleToUser(user2.UserId, role3.RoleId);

            // Assert - Check both users
            var user1Roles = _repository.ListRolesByUser(user1.UserId, _mockFactory.Object).ToList();
            var user2Roles = _repository.ListRolesByUser(user2.UserId, _mockFactory.Object).ToList();
            Assert.Equal(2, user1Roles.Count);
            Assert.Single(user2Roles);

            // Act - Remove all roles from user1
            _repository.RemoveAllRolesFromUser(user1.UserId);

            // Assert - User1 has no roles, User2 still has roles
            var user1AfterRemove = _repository.ListRolesByUser(user1.UserId, _mockFactory.Object);
            var user2AfterRemove = _repository.ListRolesByUser(user2.UserId, _mockFactory.Object);
            Assert.Empty(user1AfterRemove);
            Assert.Single(user2AfterRemove);
        }

        [Fact]
        public void RoleSharing_MultipleUsersCanHaveSameRole()
        {
            // Arrange
            var user1 = new User
            {
                Hash = "hash1",
                Name = "User 1",
                Slug = "user1",
                Email = "user1@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var user2 = new User
            {
                Hash = "hash2",
                Name = "User 2",
                Slug = "user2",
                Email = "user2@test.com",
                IsAdmin = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Users.AddRange(user1, user2);
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            _repository.AddRoleToUser(user1.UserId, role.RoleId);
            _repository.AddRoleToUser(user2.UserId, role.RoleId);

            // Assert
            Assert.True(_repository.UserHasRole(user1.UserId, role.RoleId));
            Assert.True(_repository.UserHasRole(user2.UserId, role.RoleId));

            // Act - Remove role from one user
            _repository.RemoveRoleFromUser(user1.UserId, role.RoleId);

            // Assert - Role should still exist for user2
            Assert.False(_repository.UserHasRole(user1.UserId, role.RoleId));
            Assert.True(_repository.UserHasRole(user2.UserId, role.RoleId));
        }

        #endregion
    }
}
