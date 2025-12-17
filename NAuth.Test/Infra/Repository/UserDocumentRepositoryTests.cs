using Microsoft.EntityFrameworkCore;
using Moq;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.DTO.Document;
using NAuth.Infra.Context;
using NAuth.Infra.Repository;
using Xunit;

namespace NAuth.Test.Infra.Repository
{
    public class UserDocumentRepositoryTests : IDisposable
    {
        private readonly NAuthContext _context;
        private readonly UserDocumentRepository _repository;
        private readonly Mock<IUserDocumentDomainFactory> _mockFactory;
        private readonly Mock<IUserDocumentModel> _mockDocumentModel;

        public UserDocumentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NAuthContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NAuthContext(options);
            _repository = new UserDocumentRepository(_context);
            _mockFactory = new Mock<IUserDocumentDomainFactory>();
            _mockDocumentModel = new Mock<IUserDocumentModel>();

            _mockFactory.Setup(f => f.BuildUserDocumentModel()).Returns(_mockDocumentModel.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        #region Insert Tests

        [Fact]
        public void Insert_WithValidDocument_ShouldInsertSuccessfully()
        {
            // Arrange
            _mockDocumentModel.SetupGet(m => m.DocumentId).Returns(0);
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("base64encodedstring");
            _mockDocumentModel.SetupSet(m => m.DocumentId = It.IsAny<long>());

            // Act
            var result = _repository.Insert(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedDoc = _context.UserDocuments.FirstOrDefault(d => d.UserId == 1L);
            Assert.NotNull(savedDoc);
            Assert.Equal(1L, savedDoc.UserId);
            Assert.Equal((int)DocumentTypeEnum.IdentificationDocument, savedDoc.DocumentType);
            Assert.Equal("base64encodedstring", savedDoc.Base64);
            _mockDocumentModel.VerifySet(m => m.DocumentId = It.IsAny<long>(), Times.Once);
        }

        [Fact]
        public void Insert_ShouldGenerateDocumentId()
        {
            // Arrange
            _mockDocumentModel.SetupGet(m => m.DocumentId).Returns(0);
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("testbase64");
            _mockDocumentModel.SetupSet(m => m.DocumentId = It.IsAny<long>());

            // Act
            _repository.Insert(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            var savedDoc = _context.UserDocuments.FirstOrDefault();
            Assert.NotNull(savedDoc);
            Assert.True(savedDoc.DocumentId > 0);
        }

        [Fact]
        public void Insert_MultipleDocuments_ShouldInsertAll()
        {
            // Arrange
            var mockDoc1 = new Mock<IUserDocumentModel>();
            mockDoc1.SetupGet(m => m.UserId).Returns(1L);
            mockDoc1.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockDoc1.SetupGet(m => m.Base64).Returns("doc1base64");

            var mockDoc2 = new Mock<IUserDocumentModel>();
            mockDoc2.SetupGet(m => m.UserId).Returns(1L);
            mockDoc2.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            mockDoc2.SetupGet(m => m.Base64).Returns("doc2base64");

            // Act
            _repository.Insert(mockDoc1.Object, _mockFactory.Object);
            _repository.Insert(mockDoc2.Object, _mockFactory.Object);

            // Assert
            Assert.Equal(2, _context.UserDocuments.Count());
        }

        [Fact]
        public void Insert_WithDifferentDocumentTypes_ShouldStoreCorrectly()
        {
            // Arrange
            var mockDoc1 = new Mock<IUserDocumentModel>();
            mockDoc1.SetupGet(m => m.UserId).Returns(1L);
            mockDoc1.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockDoc1.SetupGet(m => m.Base64).Returns("id-doc");

            var mockDoc2 = new Mock<IUserDocumentModel>();
            mockDoc2.SetupGet(m => m.UserId).Returns(1L);
            mockDoc2.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            mockDoc2.SetupGet(m => m.Base64).Returns("drive-licence");

            var mockDoc3 = new Mock<IUserDocumentModel>();
            mockDoc3.SetupGet(m => m.UserId).Returns(1L);
            mockDoc3.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.Unknow);
            mockDoc3.SetupGet(m => m.Base64).Returns("unknown");

            // Act
            _repository.Insert(mockDoc1.Object, _mockFactory.Object);
            _repository.Insert(mockDoc2.Object, _mockFactory.Object);
            _repository.Insert(mockDoc3.Object, _mockFactory.Object);

            // Assert
            var docs = _context.UserDocuments.ToList();
            Assert.Equal(3, docs.Count);
            Assert.Contains(docs, d => d.DocumentType == (int)DocumentTypeEnum.IdentificationDocument);
            Assert.Contains(docs, d => d.DocumentType == (int)DocumentTypeEnum.DriveLicence);
            Assert.Contains(docs, d => d.DocumentType == (int)DocumentTypeEnum.Unknow);
        }

        [Fact]
        public void Insert_WithNullUserId_ShouldInsert()
        {
            // Arrange
            _mockDocumentModel.SetupGet(m => m.UserId).Returns((long?)null);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("base64");

            // Act
            var result = _repository.Insert(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var savedDoc = _context.UserDocuments.FirstOrDefault();
            Assert.NotNull(savedDoc);
            Assert.Null(savedDoc.UserId);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithExistingDocument_ShouldUpdateSuccessfully()
        {
            // Arrange
            var document = new UserDocument
            {
                UserId = 1L,
                DocumentType = (int)DocumentTypeEnum.IdentificationDocument,
                Base64 = "originalbase64"
            };
            _context.UserDocuments.Add(document);
            _context.SaveChanges();

            _mockDocumentModel.SetupGet(m => m.DocumentId).Returns(document.DocumentId);
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("updatedbase64");

            // Act
            var result = _repository.Update(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            var updatedDoc = _context.UserDocuments.Find(document.DocumentId);
            Assert.NotNull(updatedDoc);
            Assert.Equal((int)DocumentTypeEnum.DriveLicence, updatedDoc.DocumentType);
            Assert.Equal("updatedbase64", updatedDoc.Base64);
        }

        [Fact]
        public void Update_ShouldOnlyUpdateSpecifiedDocument()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "doc1" };
            var doc2 = new UserDocument { UserId = 2L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "doc2" };
            _context.UserDocuments.AddRange(doc1, doc2);
            _context.SaveChanges();

            _mockDocumentModel.SetupGet(m => m.DocumentId).Returns(doc1.DocumentId);
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("updated-doc1");

            // Act
            _repository.Update(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            var updatedDoc1 = _context.UserDocuments.Find(doc1.DocumentId);
            var unchangedDoc2 = _context.UserDocuments.Find(doc2.DocumentId);
            Assert.Equal("updated-doc1", updatedDoc1.Base64);
            Assert.Equal("doc2", unchangedDoc2.Base64);
        }

        [Fact]
        public void Update_ChangeDocumentType_ShouldUpdate()
        {
            // Arrange
            var document = new UserDocument
            {
                UserId = 1L,
                DocumentType = (int)DocumentTypeEnum.Unknow,
                Base64 = "base64"
            };
            _context.UserDocuments.Add(document);
            _context.SaveChanges();

            _mockDocumentModel.SetupGet(m => m.DocumentId).Returns(document.DocumentId);
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("base64");

            // Act
            _repository.Update(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            var updatedDoc = _context.UserDocuments.Find(document.DocumentId);
            Assert.Equal((int)DocumentTypeEnum.IdentificationDocument, updatedDoc.DocumentType);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_WithExistingDocument_ShouldDeleteSuccessfully()
        {
            // Arrange
            var document = new UserDocument
            {
                UserId = 1L,
                DocumentType = (int)DocumentTypeEnum.IdentificationDocument,
                Base64 = "base64"
            };
            _context.UserDocuments.Add(document);
            _context.SaveChanges();
            var documentId = document.DocumentId;

            // Act
            _repository.Delete(documentId);

            // Assert
            var deletedDoc = _context.UserDocuments.Find(documentId);
            Assert.Null(deletedDoc);
            Assert.Empty(_context.UserDocuments);
        }

        [Fact]
        public void Delete_WithNonExistentDocument_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => _repository.Delete(999L));
            Assert.Null(exception);
        }

        [Fact]
        public void Delete_ShouldOnlyDeleteSpecifiedDocument()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "doc1" };
            var doc2 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "doc2" };
            _context.UserDocuments.AddRange(doc1, doc2);
            _context.SaveChanges();

            // Act
            _repository.Delete(doc1.DocumentId);

            // Assert
            var deletedDoc = _context.UserDocuments.Find(doc1.DocumentId);
            var existingDoc = _context.UserDocuments.Find(doc2.DocumentId);
            Assert.Null(deletedDoc);
            Assert.NotNull(existingDoc);
            Assert.Single(_context.UserDocuments);
        }

        [Fact]
        public void Delete_MultipleDocuments_ShouldWork()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "doc1" };
            var doc2 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "doc2" };
            _context.UserDocuments.AddRange(doc1, doc2);
            _context.SaveChanges();

            // Act
            _repository.Delete(doc1.DocumentId);
            _repository.Delete(doc2.DocumentId);

            // Assert
            Assert.Empty(_context.UserDocuments);
        }

        #endregion

        #region ListByUser Tests

        [Fact]
        public void ListByUser_WithExistingDocuments_ShouldReturnDocuments()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "doc1" };
            var doc2 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "doc2" };
            _context.UserDocuments.AddRange(doc1, doc2);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var documents = result.ToList();
            Assert.Equal(2, documents.Count);
            _mockFactory.Verify(f => f.BuildUserDocumentModel(), Times.Exactly(2));
        }

        [Fact]
        public void ListByUser_WithNoDocuments_ShouldReturnEmpty()
        {
            // Act
            var result = _repository.ListByUser(999L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ListByUser_ShouldOnlyReturnDocumentsForSpecificUser()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "user1-doc1" };
            var doc2 = new UserDocument { UserId = 2L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "user2-doc1" };
            var doc3 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "user1-doc2" };
            _context.UserDocuments.AddRange(doc1, doc2, doc3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var documents = result.ToList();
            Assert.Equal(2, documents.Count);
            _mockDocumentModel.VerifySet(m => m.Base64 = "user1-doc1", Times.Once);
            _mockDocumentModel.VerifySet(m => m.Base64 = "user1-doc2", Times.Once);
            _mockDocumentModel.VerifySet(m => m.Base64 = "user2-doc1", Times.Never);
        }

        [Fact]
        public void ListByUser_ShouldMapAllFields()
        {
            // Arrange
            var document = new UserDocument
            {
                UserId = 1L,
                DocumentType = (int)DocumentTypeEnum.IdentificationDocument,
                Base64 = "testbase64"
            };
            _context.UserDocuments.Add(document);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var documents = result.ToList();
            Assert.Single(documents);
            _mockDocumentModel.VerifySet(m => m.DocumentId = document.DocumentId, Times.Once);
            _mockDocumentModel.VerifySet(m => m.UserId = 1L, Times.Once);
            _mockDocumentModel.VerifySet(m => m.DocumentType = DocumentTypeEnum.IdentificationDocument, Times.Once);
            _mockDocumentModel.VerifySet(m => m.Base64 = "testbase64", Times.Once);
        }

        [Fact]
        public void ListByUser_WithDifferentDocumentTypes_ShouldReturnAll()
        {
            // Arrange
            var doc1 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "id" };
            var doc2 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.DriveLicence, Base64 = "drive" };
            var doc3 = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.Unknow, Base64 = "unknow" };
            _context.UserDocuments.AddRange(doc1, doc2, doc3);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            Assert.Equal(3, result.Count());
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAndListByUser_ShouldWorkTogether()
        {
            // Arrange
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns("testbase64");

            // Act
            _repository.Insert(_mockDocumentModel.Object, _mockFactory.Object);
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void InsertUpdateAndList_ShouldWorkTogether()
        {
            // Arrange - Insert
            var mockInsert = new Mock<IUserDocumentModel>();
            mockInsert.SetupGet(m => m.UserId).Returns(1L);
            mockInsert.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            mockInsert.SetupGet(m => m.Base64).Returns("original");
            long documentId = 0;
            mockInsert.SetupSet(m => m.DocumentId = It.IsAny<long>())
                .Callback<long>(id => documentId = id);

            _repository.Insert(mockInsert.Object, _mockFactory.Object);

            // Arrange - Update
            var mockUpdate = new Mock<IUserDocumentModel>();
            mockUpdate.SetupGet(m => m.DocumentId).Returns(documentId);
            mockUpdate.SetupGet(m => m.UserId).Returns(1L);
            mockUpdate.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.DriveLicence);
            mockUpdate.SetupGet(m => m.Base64).Returns("updated");

            // Act
            _repository.Update(mockUpdate.Object, _mockFactory.Object);
            var result = _repository.ListByUser(1L, _mockFactory.Object);

            // Assert
            var documents = result.ToList();
            Assert.Single(documents);
            _mockDocumentModel.VerifySet(m => m.DocumentType = DocumentTypeEnum.DriveLicence, Times.Once);
            _mockDocumentModel.VerifySet(m => m.Base64 = "updated", Times.Once);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void ListByUser_WithZeroUserId_ShouldReturnEmpty()
        {
            // Arrange
            var doc = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "test" };
            _context.UserDocuments.Add(doc);
            _context.SaveChanges();

            // Act
            var result = _repository.ListByUser(0L, _mockFactory.Object);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Delete_WithZeroId_ShouldNotThrowException()
        {
            // Arrange
            var doc = new UserDocument { UserId = 1L, DocumentType = (int)DocumentTypeEnum.IdentificationDocument, Base64 = "test" };
            _context.UserDocuments.Add(doc);
            _context.SaveChanges();

            // Act & Assert
            var exception = Record.Exception(() => _repository.Delete(0L));
            Assert.Null(exception);
            Assert.Single(_context.UserDocuments);
        }

        [Fact]
        public void Insert_WithEmptyBase64_ShouldStillInsert()
        {
            // Arrange
            _mockDocumentModel.SetupGet(m => m.UserId).Returns(1L);
            _mockDocumentModel.SetupGet(m => m.DocumentType).Returns(DocumentTypeEnum.IdentificationDocument);
            _mockDocumentModel.SetupGet(m => m.Base64).Returns(string.Empty);

            // Act
            var result = _repository.Insert(_mockDocumentModel.Object, _mockFactory.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_context.UserDocuments);
        }

        #endregion
    }
}
