using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class RoleRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly RoleRepository _repository;
        private readonly Mock<IRoleDomainFactory> _mockFactory;
        private readonly Mock<IRoleModel> _mockRoleModel;

        public RoleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new RoleRepository(_context);
            _mockFactory = new Mock<IRoleDomainFactory>();
            _mockRoleModel = new Mock<IRoleModel>();

            _mockFactory.Setup(f => f.BuildRoleModel()).Returns(_mockRoleModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Insert Tests

        [Fact]
        public void Insert_WithValidRole_ShouldInsertSuccessfully()
        {
            // Arrange
            _mockRoleModel.SetupGet(m => m.RoleId).Returns(0);
            _mockRoleModel.SetupGet(m => m.Slug).Returns("admin");
            _mockRoleModel.SetupGet(m => m.Name).Returns("Administrator");
            _mockRoleModel.SetupSet(m => m.RoleId = It.IsAny<long>());

            // Act
            var result = _repository.Insert(_mockRoleModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedRole = _context.Roles.FirstOrDefault(r => r.Slug == "admin");
            Assert.NotNull(savedRole);
            Assert.Equal("admin", savedRole.Slug);
            Assert.Equal("Administrator", savedRole.Name);
            _mockRoleModel.VerifySet(m => m.RoleId = It.IsAny<long>(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldGenerateRoleId()
        {
            // Arrange
            _mockRoleModel.SetupGet(m => m.RoleId).Returns(0);
            _mockRoleModel.SetupGet(m => m.Slug).Returns("user");
            _mockRoleModel.SetupGet(m => m.Name).Returns("User");
            _mockRoleModel.SetupSet(m => m.RoleId = It.IsAny<long>());

            // Act
            _repository.Insert(_mockRoleModel.Object, _mockFactory.Object);

            // Assert
            var savedRole = _context.Roles.FirstOrDefault(r => r.Slug == "user");
            Assert.NotNull(savedRole);
            Assert.True(savedRole.RoleId > 0);
        }

        [Fact]
        public void Insert_MultipleRoles_ShouldInsertAll()
        {
            // Arrange
            var mockRole1 = new Mock<IRoleModel>();
            mockRole1.SetupGet(m => m.Slug).Returns("admin");
            mockRole1.SetupGet(m => m.Name).Returns("Administrator");

            var mockRole2 = new Mock<IRoleModel>();
            mockRole2.SetupGet(m => m.Slug).Returns("user");
            mockRole2.SetupGet(m => m.Name).Returns("User");

            // Act
            _repository.Insert(mockRole1.Object, _mockFactory.Object);
            _repository.Insert(mockRole2.Object, _mockFactory.Object);

            // Assert
            Assert.Equal(2, _context.Roles.Count());
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithExistingRole_ShouldUpdateSuccessfully()
        {
            // Arrange
            var role = new Role
            {
                Slug = "admin",
                Name = "Administrator"
            };
            _context.Roles.Add(role);
            _context.SaveChanges();

            _mockRoleModel.SetupGet(m => m.RoleId).Returns(role.RoleId);
            _mockRoleModel.SetupGet(m => m.Slug).Returns("super-admin");
            _mockRoleModel.SetupGet(m => m.Name).Returns("Super Administrator");

            // Act
            var result = _repository.Update(_mockRoleModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var updatedRole = _context.Roles.Find(role.RoleId);
            Assert.NotNull(updatedRole);
            Assert.Equal("super-admin", updatedRole.Slug);
            Assert.Equal("Super Administrator", updatedRole.Name);
        }

        [Fact]
        public void Update_ShouldOnlyUpdateSpecifiedRole()
        {
            // Arrange
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            _context.Roles.AddRange(role1, role2);
            _context.SaveChanges();

            _mockRoleModel.SetupGet(m => m.RoleId).Returns(role1.RoleId);
            _mockRoleModel.SetupGet(m => m.Slug).Returns("super-admin");
            _mockRoleModel.SetupGet(m => m.Name).Returns("Super Administrator");

            // Act
            _repository.Update(_mockRoleModel.Object, _mockFactory.Object);

            // Assert
            var updatedRole1 = _context.Roles.Find(role1.RoleId);
            var unchangedRole2 = _context.Roles.Find(role2.RoleId);
            Assert.Equal("super-admin", updatedRole1.Slug);
            Assert.Equal("user", unchangedRole2.Slug);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithExistingId_ShouldReturnRole()
        {
            // Arrange
            var role = new Role
            {
                Slug = "admin",
                Name = "Administrator"
            };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.GetById(role.RoleId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRoleModel.VerifySet(m => m.RoleId = role.RoleId, Times.Once);
            _mockRoleModel.VerifySet(m => m.Slug = "admin", Times.Once);
            _mockRoleModel.VerifySet(m => m.Name = "Administrator", Times.Once);
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetById(999L, _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetById_WithMultipleRoles_ShouldReturnCorrectRole()
        {
            // Arrange
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            _context.Roles.AddRange(role1, role2);
            _context.SaveChanges();

            // Act
            var result = _repository.GetById(role2.RoleId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRoleModel.VerifySet(m => m.Slug = "user", Times.Once);
        }

        #endregion

        #region GetBySlug Tests

        [Fact]
        public void GetBySlug_WithExistingSlug_ShouldReturnRole()
        {
            // Arrange
            var role = new Role
            {
                Slug = "admin",
                Name = "Administrator"
            };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.GetBySlug("admin", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRoleModel.VerifySet(m => m.RoleId = role.RoleId, Times.Once);
            _mockRoleModel.VerifySet(m => m.Slug = "admin", Times.Once);
            _mockRoleModel.VerifySet(m => m.Name = "Administrator", Times.Once);
        }

        [Fact]
        public void GetBySlug_WithNonExistentSlug_ShouldReturnNull()
        {
            // Act
            var result = _repository.GetBySlug("nonexistent", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetBySlug_WithMultipleRoles_ShouldReturnCorrectRole()
        {
            // Arrange
            var role1 = new Role { Slug = "admin", Name = "Administrator" };
            var role2 = new Role { Slug = "user", Name = "User" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();

            // Act
            var result = _repository.GetBySlug("moderator", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRoleModel.VerifySet(m => m.Slug = "moderator", Times.Once);
            _mockRoleModel.VerifySet(m => m.Name = "Moderator", Times.Once);
        }

        [Fact]
        public void GetBySlug_IsCaseSensitive()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.GetBySlug("Admin", _mockFactory.Object);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ExistSlug Tests

        [Fact]
        public void ExistSlug_WithExistingSlug_ShouldReturnTrue()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(0, "admin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ExistSlug_WithNonExistentSlug_ShouldReturnFalse()
        {
            // Act
            var result = _repository.ExistSlug(0, "nonexistent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExistSlug_WithSameRoleId_ShouldReturnFalse()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(role.RoleId, "admin");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExistSlug_WithDifferentRoleId_ShouldReturnTrue()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(999L, "admin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ExistSlug_WithNewRole_ShouldCheckDuplicates()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act - checking for new role (roleId = 0)
            var result = _repository.ExistSlug(0, "admin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ExistSlug_IsCaseSensitive()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.ExistSlug(0, "Admin");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ListRoles Tests

        [Fact]
        public void ListRoles_WithMultipleRoles_ShouldReturnOrderedByName()
        {
            // Arrange
            var role1 = new Role { Slug = "user", Name = "User" };
            var role2 = new Role { Slug = "admin", Name = "Administrator" };
            var role3 = new Role { Slug = "moderator", Name = "Moderator" };
            _context.Roles.AddRange(role1, role2, role3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListRoles(10, _mockFactory.Object);

            // Assert
            var roleList = result.ToList();
            Assert.Equal(3, roleList.Count);
            _mockFactory.Verify(f => f.BuildRoleModel(), Times.Exactly(3));
        }

        [Fact]
        public void ListRoles_ShouldRespectTakeParameter()
        {
            // Arrange
            var roles = new[]
            {
                new Role { Slug = "admin", Name = "Administrator" },
                new Role { Slug = "user", Name = "User" },
                new Role { Slug = "moderator", Name = "Moderator" },
                new Role { Slug = "guest", Name = "Guest" },
                new Role { Slug = "editor", Name = "Editor" }
            };
            _context.Roles.AddRange(roles);
            _context.SaveChanges();

            // Act
            var result = _repository.ListRoles(3, _mockFactory.Object);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ListRoles_WithEmptyDatabase_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListRoles(10, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListRoles_ShouldOrderAlphabetically()
        {
            // Arrange
            var roles = new[]
            {
                new Role { Slug = "zebra", Name = "Zebra Role" },
                new Role { Slug = "alpha", Name = "Alpha Role" },
                new Role { Slug = "beta", Name = "Beta Role" }
            };
            _context.Roles.AddRange(roles);
            _context.SaveChanges();

            var orderedNames = new List<string>();
            _mockRoleModel.SetupSet(m => m.Name = It.IsAny<string>())
                .Callback<string>(name => orderedNames.Add(name));

            // Act
            var result = _repository.ListRoles(10, _mockFactory.Object).ToList();

            // Assert
            Assert.Equal("Alpha Role", orderedNames[0]);
            Assert.Equal("Beta Role", orderedNames[1]);
            Assert.Equal("Zebra Role", orderedNames[2]);
        }

        [Fact]
        public void ListRoles_WithTakeZero_ShouldReturnEmpty()
        {
            // Arrange
            var role = new Role { Slug = "admin", Name = "Administrator" };
            _context.Roles.Add(role);
            _context.SaveChanges();

            // Act
            var result = _repository.ListRoles(0, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAndGetById_ShouldWorkTogether()
        {
            // Arrange
            _mockRoleModel.SetupGet(m => m.Slug).Returns("admin");
            _mockRoleModel.SetupGet(m => m.Name).Returns("Administrator");
            long roleId = 0;
            _mockRoleModel.SetupSet(m => m.RoleId = It.IsAny<long>())
                .Callback<long>(id => roleId = id);

            // Act
            _repository.Insert(_mockRoleModel.Object, _mockFactory.Object);
            var result = _repository.GetById(roleId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void InsertAndGetBySlug_ShouldWorkTogether()
        {
            // Arrange
            _mockRoleModel.SetupGet(m => m.Slug).Returns("admin");
            _mockRoleModel.SetupGet(m => m.Name).Returns("Administrator");

            // Act
            _repository.Insert(_mockRoleModel.Object, _mockFactory.Object);
            var result = _repository.GetBySlug("admin", _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void InsertUpdateAndGet_ShouldWorkTogether()
        {
            // Arrange - Insert
            var mockInsert = new Mock<IRoleModel>();
            mockInsert.SetupGet(m => m.Slug).Returns("admin");
            mockInsert.SetupGet(m => m.Name).Returns("Administrator");
            long roleId = 0;
            mockInsert.SetupSet(m => m.RoleId = It.IsAny<long>())
                .Callback<long>(id => roleId = id);

            _repository.Insert(mockInsert.Object, _mockFactory.Object);

            // Arrange - Update
            var mockUpdate = new Mock<IRoleModel>();
            mockUpdate.SetupGet(m => m.RoleId).Returns(roleId);
            mockUpdate.SetupGet(m => m.Slug).Returns("super-admin");
            mockUpdate.SetupGet(m => m.Name).Returns("Super Administrator");

            // Act
            _repository.Update(mockUpdate.Object, _mockFactory.Object);
            var result = _repository.GetById(roleId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRoleModel.VerifySet(m => m.Slug = "super-admin", Times.Once);
            _mockRoleModel.VerifySet(m => m.Name = "Super Administrator", Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_InsertUpdateCheckExistList()
        {
            // Insert
            var mockRole = new Mock<IRoleModel>();
            mockRole.SetupGet(m => m.Slug).Returns("admin");
            mockRole.SetupGet(m => m.Name).Returns("Administrator");
            long roleId = 0;
            mockRole.SetupSet(m => m.RoleId = It.IsAny<long>())
                .Callback<long>(id => roleId = id);

            _repository.Insert(mockRole.Object, _mockFactory.Object);

            // Check existence
            var exists = _repository.ExistSlug(0, "admin");
            Assert.True(exists);

            // List
            var roles = _repository.ListRoles(10, _mockFactory.Object);
            Assert.Single(roles);

            // Get by slug
            var retrieved = _repository.GetBySlug("admin", _mockFactory.Object);
            Assert.NotNull(retrieved);
        }

        #endregion
    }
}
