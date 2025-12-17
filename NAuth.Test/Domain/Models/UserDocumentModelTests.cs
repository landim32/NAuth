using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.DTO.Document;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using Xunit;

namespace NAuth.Test.Domain.Models
{
    public class UserDocumentModelTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>> _mockRepository;
        private readonly Mock<IUserDocumentDomainFactory> _mockFactory;
        private readonly UserDocumentModel _documentModel;

        public UserDocumentModelTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepository = new Mock<IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>>();
            _mockFactory = new Mock<IUserDocumentDomainFactory>();
            _documentModel = new UserDocumentModel(_mockUnitOfWork.Object, _mockRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var model = new UserDocumentModel(_mockUnitOfWork.Object, _mockRepository.Object);

            // Assert
            Assert.NotNull(model);
        }

        #endregion

        #region Property Tests

        [Fact]
        public void Properties_ShouldGetAndSetValues()
        {
            // Arrange
            var documentId = 1L;
            var userId = 100L;
            var documentType = DocumentTypeEnum.IdentificationDocument;
            var base64 = "base64encodedstring";

            // Act
            _documentModel.DocumentId = documentId;
            _documentModel.UserId = userId;
            _documentModel.DocumentType = documentType;
            _documentModel.Base64 = base64;

            // Assert
            Assert.Equal(documentId, _documentModel.DocumentId);
            Assert.Equal(userId, _documentModel.UserId);
            Assert.Equal(documentType, _documentModel.DocumentType);
            Assert.Equal(base64, _documentModel.Base64);
        }

        [Fact]
        public void UserId_WithNullValue_ShouldAllowNull()
        {
            // Arrange & Act
            _documentModel.UserId = null;

            // Assert
            Assert.Null(_documentModel.UserId);
        }

        [Fact]
        public void DocumentType_WithAllEnumValues_ShouldHandleCorrectly()
        {
            // Act & Assert
            _documentModel.DocumentType = DocumentTypeEnum.Unknow;
            Assert.Equal(DocumentTypeEnum.Unknow, _documentModel.DocumentType);

            _documentModel.DocumentType = DocumentTypeEnum.IdentificationDocument;
            Assert.Equal(DocumentTypeEnum.IdentificationDocument, _documentModel.DocumentType);

            _documentModel.DocumentType = DocumentTypeEnum.DriveLicence;
            Assert.Equal(DocumentTypeEnum.DriveLicence, _documentModel.DocumentType);
        }

        [Fact]
        public void Base64_WithEmptyString_ShouldAllowEmptyString()
        {
            // Arrange & Act
            _documentModel.Base64 = "";

            // Assert
            Assert.Equal("", _documentModel.Base64);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_ShouldCallRepositoryInsert()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Insert(mockModel.Object, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Insert(mockModel.Object, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Insert_WithDocumentData_ShouldPassCorrectData()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            mockModel.SetupGet(m => m.DocumentId).Returns(0);
            mockModel.SetupGet(m => m.UserId).Returns(1L);
            mockModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockModel.SetupGet(m => m.Base64).Returns("test-base64");

            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Insert(It.IsAny<IUserDocumentModel>(), _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(
                It.Is<IUserDocumentModel>(m => 
                    m.UserId == 1L &&
                    m.DocumentType == DocumentTypeEnum.IdentificationDocument &&
                    m.Base64 == "test-base64"),
                _mockFactory.Object),
                Times.Once);
        }

        [Fact]
        public void Insert_WithDifferentDocumentTypes_ShouldHandleAll()
        {
            // Arrange
            var mockModel1 = new Mock<IUserDocumentModel>();
            mockModel1.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.Unknow);

            var mockModel2 = new Mock<IUserDocumentModel>();
            mockModel2.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);

            var mockModel3 = new Mock<IUserDocumentModel>();
            mockModel3.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);

            var mockReturn = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Insert(It.IsAny<IUserDocumentModel>(), _mockFactory.Object))
                .Returns(mockReturn.Object);

            // Act
            var result1 = _documentModel.Insert(mockModel1.Object, _mockFactory.Object);
            var result2 = _documentModel.Insert(mockModel2.Object, _mockFactory.Object);
            var result3 = _documentModel.Insert(mockModel3.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            _mockRepository.Verify(r => r.Insert(It.IsAny<IUserDocumentModel>(), _mockFactory.Object), Times.Exactly(3));
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Update(mockModel.Object, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _documentModel.Update(mockModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mockReturnModel.Object, result);
            _mockRepository.Verify(r => r.Update(mockModel.Object, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Update_WithModifiedData_ShouldPassCorrectData()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            mockModel.SetupGet(m => m.DocumentId).Returns(1L);
            mockModel.SetupGet(m => m.UserId).Returns(100L);
            mockModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            mockModel.SetupGet(m => m.Base64).Returns("updated-base64");

            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Update(It.IsAny<IUserDocumentModel>(), _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _documentModel.Update(mockModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Update(
                It.Is<IUserDocumentModel>(m => 
                    m.DocumentId == 1L &&
                    m.UserId == 100L &&
                    m.DocumentType == DocumentTypeEnum.DriveLicence &&
                    m.Base64 == "updated-base64"),
                _mockFactory.Object),
                Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_ShouldCallRepositoryDelete()
        {
            // Arrange
            var documentId = 1L;
            _mockRepository.Setup(r => r.Delete(documentId));

            // Act
            _documentModel.Delete(documentId);

            // Assert
            _mockRepository.Verify(r => r.Delete(documentId), Times.Once);
        }

        [Fact]
        public void Delete_WithZeroId_ShouldStillCallRepository()
        {
            // Arrange
            var documentId = 0L;
            _mockRepository.Setup(r => r.Delete(documentId));

            // Act
            _documentModel.Delete(documentId);

            // Assert
            _mockRepository.Verify(r => r.Delete(documentId), Times.Once);
        }

        [Fact]
        public void Delete_WithNegativeId_ShouldStillCallRepository()
        {
            // Arrange
            var documentId = -1L;
            _mockRepository.Setup(r => r.Delete(documentId));

            // Act
            _documentModel.Delete(documentId);

            // Assert
            _mockRepository.Verify(r => r.Delete(documentId), Times.Once);
        }

        [Fact]
        public void Delete_MultipleCallsWithSameId_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var documentId = 1L;
            _mockRepository.Setup(r => r.Delete(documentId));

            // Act
            _documentModel.Delete(documentId);
            _documentModel.Delete(documentId);
            _documentModel.Delete(documentId);

            // Assert
            _mockRepository.Verify(r => r.Delete(documentId), Times.Exactly(3));
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingDocuments_ShouldReturnList()
        {
            // Arrange
            var userId = 1L;
            var mockDocuments = new List<IUserDocumentModel>
            {
                Mock.Of<IUserDocumentModel>(),
                Mock.Of<IUserDocumentModel>(),
                Mock.Of<IUserDocumentModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act
            var result = _documentModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_WithNoDocuments_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = 999L;
            var mockDocuments = new List<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act
            var result = _documentModel.ListByUser(userId, _mockFactory.Object);

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
            var mockDocuments = new List<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act
            var result = _documentModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ListByUser_ShouldReturnIEnumerable()
        {
            // Arrange
            var userId = 1L;
            var mockDocuments = new List<IUserDocumentModel>
            {
                Mock.Of<IUserDocumentModel>()
            };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act
            var result = _documentModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<IUserDocumentModel>>(result);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CompleteWorkflow_InsertAndList()
        {
            // Arrange
            var userId = 1L;
            var mockModel = new Mock<IUserDocumentModel>();
            mockModel.SetupGet(m => m.UserId).Returns(userId);
            mockModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockModel.SetupGet(m => m.Base64).Returns("test-document");

            var insertedModel = new Mock<IUserDocumentModel>();
            insertedModel.SetupGet(m => m.DocumentId).Returns(1L);
            insertedModel.SetupGet(m => m.UserId).Returns(userId);

            _mockRepository
                .Setup(r => r.Insert(mockModel.Object, _mockFactory.Object))
                .Returns(insertedModel.Object);

            var mockDocuments = new List<IUserDocumentModel> { insertedModel.Object };
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act - Insert
            var insertResult = _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Act - List
            var listResult = _documentModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(listResult);
            Assert.Single(listResult);
            _mockRepository.Verify(r => r.Insert(mockModel.Object, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void CompleteWorkflow_InsertUpdateAndDelete()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            mockModel.SetupGet(m => m.DocumentId).Returns(1L);
            mockModel.SetupGet(m => m.UserId).Returns(1L);
            mockModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);

            var insertedModel = new Mock<IUserDocumentModel>();
            insertedModel.SetupGet(m => m.DocumentId).Returns(1L);

            var updatedModel = new Mock<IUserDocumentModel>();
            updatedModel.SetupGet(m => m.DocumentId).Returns(1L);

            _mockRepository
                .Setup(r => r.Insert(mockModel.Object, _mockFactory.Object))
                .Returns(insertedModel.Object);

            _mockRepository
                .Setup(r => r.Update(mockModel.Object, _mockFactory.Object))
                .Returns(updatedModel.Object);

            _mockRepository.Setup(r => r.Delete(1L));

            // Act - Insert
            var insertResult = _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Act - Update
            var updateResult = _documentModel.Update(mockModel.Object, _mockFactory.Object);

            // Act - Delete
            _documentModel.Delete(1L);

            // Assert
            Assert.NotNull(insertResult);
            Assert.NotNull(updateResult);
            _mockRepository.Verify(r => r.Insert(mockModel.Object, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Update(mockModel.Object, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Delete(1L), Times.Once);
        }

        [Fact]
        public void MultipleDocuments_ForSameUser_ShouldWorkIndependently()
        {
            // Arrange
            var userId = 1L;

            var doc1 = new Mock<IUserDocumentModel>();
            doc1.SetupGet(m => m.UserId).Returns(userId);
            doc1.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);

            var doc2 = new Mock<IUserDocumentModel>();
            doc2.SetupGet(m => m.UserId).Returns(userId);
            doc2.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);

            var mockReturn1 = Mock.Of<IUserDocumentModel>();
            var mockReturn2 = Mock.Of<IUserDocumentModel>();

            _mockRepository
                .Setup(r => r.Insert(doc1.Object, _mockFactory.Object))
                .Returns(mockReturn1);

            _mockRepository
                .Setup(r => r.Insert(doc2.Object, _mockFactory.Object))
                .Returns(mockReturn2);

            // Act
            var result1 = _documentModel.Insert(doc1.Object, _mockFactory.Object);
            var result2 = _documentModel.Insert(doc2.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            _mockRepository.Verify(r => r.Insert(doc1.Object, _mockFactory.Object), Times.Once);
            _mockRepository.Verify(r => r.Insert(doc2.Object, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void ModelState_ShouldMaintainValuesBetweenOperations()
        {
            // Arrange
            var userId = 1L;
            _documentModel.UserId = userId;
            _documentModel.DocumentType = DocumentTypeEnum.IdentificationDocument;
            _documentModel.Base64 = "test-base64";

            var mockModel = new Mock<IUserDocumentModel>();
            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Insert(mockModel.Object, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Assert - Values should remain unchanged after operation
            Assert.Equal(userId, _documentModel.UserId);
            Assert.Equal(DocumentTypeEnum.IdentificationDocument, _documentModel.DocumentType);
            Assert.Equal("test-base64", _documentModel.Base64);
        }

        [Fact]
        public void ListByUser_CalledMultipleTimes_ShouldInvokeRepositoryEachTime()
        {
            // Arrange
            var userId = 1L;
            var mockDocuments = new List<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.ListByUser(userId, _mockFactory.Object))
                .Returns(mockDocuments);

            // Act
            _documentModel.ListByUser(userId, _mockFactory.Object);
            _documentModel.ListByUser(userId, _mockFactory.Object);
            _documentModel.ListByUser(userId, _mockFactory.Object);

            // Assert
            _mockRepository.Verify(r => r.ListByUser(userId, _mockFactory.Object), Times.Exactly(3));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void Insert_WithNullUserId_ShouldNotThrowException()
        {
            // Arrange
            var mockModel = new Mock<IUserDocumentModel>();
            mockModel.SetupGet(m => m.UserId).Returns((long?)null);
            mockModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockModel.SetupGet(m => m.Base64).Returns("test");

            var mockReturnModel = new Mock<IUserDocumentModel>();
            _mockRepository
                .Setup(r => r.Insert(mockModel.Object, _mockFactory.Object))
                .Returns(mockReturnModel.Object);

            // Act
            var result = _documentModel.Insert(mockModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.Insert(mockModel.Object, _mockFactory.Object), Times.Once);
        }

        [Fact]
        public void Properties_WithNullBase64_ShouldAllowNull()
        {
            // Arrange & Act
            _documentModel.Base64 = null;

            // Assert
            Assert.Null(_documentModel.Base64);
        }

        [Fact]
        public void Base64_WithLargeString_ShouldHandleCorrectly()
        {
            // Arrange
            var largeBase64 = new string('A', 10000);

            // Act
            _documentModel.Base64 = largeBase64;

            // Assert
            Assert.Equal(largeBase64, _documentModel.Base64);
            Assert.Equal(10000, _documentModel.Base64.Length);
        }

        [Fact]
        public void DocumentType_WithInvalidEnumValue_ShouldStillStore()
        {
            // Arrange
            var invalidEnumValue = (DocumentTypeEnum)999;

            // Act
            _documentModel.DocumentType = invalidEnumValue;

            // Assert
            Assert.Equal(invalidEnumValue, _documentModel.DocumentType);
        }

        #endregion
    }
}
