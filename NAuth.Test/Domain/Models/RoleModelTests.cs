using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using Xunit;

namespace NAuth.Test.Domain.Models
{
    public class RoleModelTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRoleRepository<IRoleModel, IRoleDomainFactory>> _mockRepository;
        private readonly Mock<IRoleDomainFactory> _mockFactory;
        private readonly RoleModel _roleModel;

        public RoleModelTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IRoleRepository<IRoleModel, IRoleDomainFactory>>();
            _mockFactory = new Mock<IRoleDomainFactory>();
            _roleModel = new RoleModel(_mockUnitOfWork.Object, _mockRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var model = new RoleModel(_mockUnitOfWork.Object, _mockRepository.Object);

            // Assert
            Assert.NotNull(model);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_ShouldGetAndSetValues()
        {
            // Arrange
            var roleId = 1L;
            var slug = "admin";
            var name = "Administrator";

            // Act
            _roleModel.RoleId = roleId;
            _roleModel.Slug = slug;
            _roleModel.Name = name;

            // Assert
            Assert.Equal(roleId, _roleModel.RoleId);
            Assert.Equal(slug, _roleModel.Slug);
            Assert.Equal(name, _roleModel.Name);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_ShouldCallRepositoryInsert()
        {
            // Arrange
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.Insert(_roleModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Insert(_roleModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Insert_WithRoleData_ShouldPassCorrectData()
        {
            // Arrange
            _roleModel.Slug = "test-slug";
            _roleModel.Name = "Test Role";
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.Insert(_roleModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.Insert(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(
                It.Is<IRoleModel>(m => m.Slug == "test-slug" && m.Name == "Test Role"),
                _mockFactory.Object),
                Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.Update(_roleModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.Update(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Update(_roleModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Update_WithModifiedData_ShouldPassCorrectData()
        {
            // Arrange
            _roleModel.RoleId = 1L;
            _roleModel.Slug = "updated-slug";
            _roleModel.Name = "Updated Role";
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.Update(_roleModel, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.Update(_mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Update(
                It.Is<IRoleModel>(m => 
                    m.RoleId == 1L && 
                    m.Slug == "updated-slug" && 
                    m.Name == "Updated Role"),
                _mockFactory.Object),
                Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithExistingId_ShouldReturnRole()
        {
            // Arrange
            var roleId = 1L;
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.GetById(roleId, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.GetById(roleId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.GetById(roleId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void GetById_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var roleId = 999L;
            _mockRepository
                .Setup(r => r.GetById(roleId, _mockFactory.Object))
                .Returns((IRoleModel)null!);

            // Act
            var result = _roleModel.GetById(roleId, _mockFactory.Object);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetById(roleId, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region GetBySlug Tests

        [Fact]
        public void GetBySlug_WithExistingSlug_ShouldReturnRole()
        {
            // Arrange
            var slug = "admin";
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.GetBySlug(slug, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _roleModel.GetBySlug(slug, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.GetBySlug(slug, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void GetBySlug_WithNonExistentSlug_ShouldReturnNull()
        {
            // Arrange
            var slug = "nonexistent";
            _mockRepository
                .Setup(r => r.GetBySlug(slug, _mockFactory.Object))
                .Returns((IRoleModel)null!);

            // Act
            var result = _roleModel.GetBySlug(slug, _mockFactory.Object);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetBySlug(slug, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void GetBySlug_WithEmptySlug_ShouldCallRepository()
        {
            // Arrange
            var slug = "";
            _mockRepository
                .Setup(r => r.GetBySlug(slug, _mockFactory.Object))
                .Returns((IRoleModel)null!);

            // Act
            var result = _roleModel.GetBySlug(slug, _mockFactory.Object);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetBySlug(slug, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region ListRoles Tests

        [Fact]
        public void ListRoles_WithTake_ShouldReturnRoles()
        {
            // Arrange
            var take = 10;
            var mockRoles = new List<IRoleModel>
            {
                Mock.Of<IRoleModel>(),
                Mock.Of<IRoleModel>(),
                Mock.Of<IRoleModel>()
            };
            _mockRepository
                .Setup(r => r.ListRoles(take, _mockFactory.Object))
                .Returns(mockRoles);

            // Act
            var result = _roleModel.ListRoles(take, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockRepository.Verify(r => r.ListRoles(take, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListRoles_WithZeroTake_ShouldCallRepository()
        {
            // Arrange
            var take = 0;
            var mockRoles = new List<IRoleModel>();
            _mockRepository
                .Setup(r => r.ListRoles(take, _mockFactory.Object))
                .Returns(mockRoles);

            // Act
            var result = _roleModel.ListRoles(take, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.ListRoles(take, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListRoles_WithNegativeTake_ShouldCallRepository()
        {
            // Arrange
            var take = -1;
            var mockRoles = new List<IRoleModel>();
            _mockRepository
                .Setup(r => r.ListRoles(take, _mockFactory.Object))
                .Returns(mockRoles);

            // Act
            var result = _roleModel.ListRoles(take, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.ListRoles(take, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListRoles_WhenEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var take = 10;
            var mockRoles = new List<IRoleModel>();
            _mockRepository
                .Setup(r => r.ListRoles(take, _mockFactory.Object))
                .Returns(mockRoles);

            // Act
            var result = _roleModel.ListRoles(take, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockRepository.Verify(r => r.ListRoles(take, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region ExistSlug Tests

        [Fact]
        public void ExistSlug_WithExistingSlug_ShouldReturnTrue()
        {
            // Arrange
            var roleId = 0L;
            var slug = "admin";
            _mockRepository
                .Setup(r => r.ExistSlug(roleId, slug))
                .Returns(true);

            // Act
            var result = _roleModel.ExistSlug(roleId, slug);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.ExistSlug(roleId, slug), Times.Once);
        }

        [Fact]
        public void ExistSlug_WithNonExistentSlug_ShouldReturnFalse()
        {
            // Arrange
            var roleId = 0L;
            var slug = "nonexistent";
            _mockRepository
                .Setup(r => r.ExistSlug(roleId, slug))
                .Returns(false);

            // Act
            var result = _roleModel.ExistSlug(roleId, slug);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.ExistSlug(roleId, slug), Times.Once);
        }

        [Fact]
        public void ExistSlug_WithSameRoleId_ShouldReturnFalse()
        {
            // Arrange
            var roleId = 1L;
            var slug = "admin";
            _mockRepository
                .Setup(r => r.ExistSlug(roleId, slug))
                .Returns(false);

            // Act
            var result = _roleModel.ExistSlug(roleId, slug);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.ExistSlug(roleId, slug), Times.Once);
        }

        [Fact]
        public void ExistSlug_WithDifferentRoleId_ShouldReturnTrue()
        {
            // Arrange
            var roleId = 999L;
            var slug = "admin";
            _mockRepository
                .Setup(r => r.ExistSlug(roleId, slug))
                .Returns(true);

            // Act
            var result = _roleModel.ExistSlug(roleId, slug);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.ExistSlug(roleId, slug), Times.Once);
        }

        [Fact]
        public void ExistSlug_WithEmptySlug_ShouldCallRepository()
        {
            // Arrange
            var roleId = 0L;
            var slug = "";
            _mockRepository
                .Setup(r => r.ExistSlug(roleId, slug))
                .Returns(false);

            // Act
            var result = _roleModel.ExistSlug(roleId, slug);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.ExistSlug(roleId, slug), Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CompleteWorkflow_InsertAndRetrieve()
        {
            // Arrange
            _roleModel.Slug = "test-role";
            _roleModel.Name = "Test Role";
            
            var insertedModel = new Mock<IRoleModel>();
            insertedModel.SetupGet(m => m.RoleId).Returns(1L);
            insertedModel.SetupGet(m => m.Slug).Returns("test-role");
            insertedModel.SetupGet(m => m.Name).Returns("Test Role");
            
            _mockRepository
                .Setup(r => r.Insert(_roleModel, _mockFactory.Object))
                .Returns(insertedModel.Object);
            
            _mockRepository
                .Setup(r => r.GetById(1L, _mockFactory.Object))
                .Returns(insertedModel.Object);

            // Act - Insert
            var insertResult = _roleModel.Insert(_mockFactory.Object);
            
            // Act - Retrieve
            var getResult = _roleModel.GetById(1L, _mockFactory.Object);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(getResult);
            Assert.Equal(1L, getResult.RoleId);
            Assert.Equal("test-role", getResult.Slug);
            _mockRepository.Verify(r => r.Insert(_roleModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.GetById(1L, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_InsertUpdateAndRetrieve()
        {
            // Arrange
            _roleModel.Slug = "original-slug";
            _roleModel.Name = "Original Name";
            
            var insertedModel = new Mock<IRoleModel>();
            insertedModel.SetupGet(m => m.RoleId).Returns(1L);
            insertedModel.SetupGet(m => m.Slug).Returns("original-slug");
            insertedModel.SetupGet(m => m.Name).Returns("Original Name");
            
            var updatedModel = new Mock<IRoleModel>();
            updatedModel.SetupGet(m => m.RoleId).Returns(1L);
            updatedModel.SetupGet(m => m.Slug).Returns("updated-slug");
            updatedModel.SetupGet(m => m.Name).Returns("Updated Name");
            
            _mockRepository
                .Setup(r => r.Insert(_roleModel, _mockFactory.Object))
                .Returns(insertedModel.Object);
            
            _mockRepository
                .Setup(r => r.Update(It.IsAny<IRoleModel>(), _mockFactory.Object))
                .Returns(updatedModel.Object);
            
            _mockRepository
                .Setup(r => r.GetById(1L, _mockFactory.Object))
                .Returns(updatedModel.Object);

            // Act - Insert
            var insertResult = _roleModel.Insert(_mockFactory.Object);
            
            // Act - Update
            _roleModel.RoleId = 1L;
            _roleModel.Slug = "updated-slug";
            _roleModel.Name = "Updated Name";
            var updateResult = _roleModel.Update(_mockFactory.Object);
            
            // Act - Retrieve
            var getResult = _roleModel.GetById(1L, _mockFactory.Object);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(updateResult);
            Assert.NotNull(getResult);
            Assert.Equal("updated-slug", getResult.Slug);
            Assert.Equal("Updated Name", getResult.Name);
            _mockRepository.Verify(r => r.Insert(_roleModel, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Update(It.IsAny<IRoleModel>(), _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.GetById(1L, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_CheckSlugExistenceBeforeInsert()
        {
            // Arrange
            var slug = "new-role";
            _roleModel.Slug = slug;
            _roleModel.Name = "New Role";
            
            _mockRepository
                .Setup(r => r.ExistSlug(0L, slug))
                .Returns(false);
            
            var insertedModel = new Mock<IRoleModel>();
            insertedModel.SetupGet(m => m.RoleId).Returns(1L);
            _mockRepository
                .Setup(r => r.Insert(_roleModel, _mockFactory.Object))
                .Returns(insertedModel.Object);

            // Act - Check slug existence
            var slugExists = _roleModel.ExistSlug(0L, slug);
            
            // Act - Insert if slug doesn't exist
            IRoleModel result = null!;
            if (!slugExists)
            {
                result = _roleModel.Insert(_mockFactory.Object);
            }

            // Assert
            Assert.False(slugExists);
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.ExistSlug(0L, slug), Times.Once);
            _mockRepository.Verify(r => r.Insert(_roleModel, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void MultipleCallsToSameMethod_ShouldInvokeRepositoryMultipleTimes()
        {
            // Arrange
            var roleId = 1L;
            var mockReturnModel = new Mock<IRoleModel>();
            _mockRepository
                .Setup(r => r.GetById(roleId, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            _roleModel.GetById(roleId, _mockFactory.Object);
            _roleModel.GetById(roleId, _mockFactory.Object);
            _roleModel.GetById(roleId, _mockFactory.Object);

            // Assert
            _mockRepository.Verify(r => r.GetById(roleId, _mockFactory.Object), Times.Exactly(3));
        }

        #endregion
    }
}
