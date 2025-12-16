using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.DTO.Document;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using System.Collections.Generic;

namespace NAuth.Domain.Models
{
    public class UserDocumentModel : IUserDocumentModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> _repositoryDocument;

        public UserDocumentModel(IUnitOfWork unitOfWork, IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> repositoryDocument)
        {
            _unitOfWork = unitOfWork;
            _repositoryDocument = repositoryDocument;
        }

        public long DocumentId { get; set; }
        public long? UserId { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public string Base64 { get; set; }

        public void Delete(long documentId)
        {
            _repositoryDocument.Delete(documentId);
        }

        public IUserDocumentModel Insert(IUserDocumentModel model, IUserDocumentDomainFactory factory)
        {
            return _repositoryDocument.Insert(model, factory);
        }

        public IEnumerable<IUserDocumentModel> ListByUser(long userId, IUserDocumentDomainFactory factory)
        {
            return _repositoryDocument.ListByUser(userId, factory);
        }

        public IUserDocumentModel Update(IUserDocumentModel model, IUserDocumentDomainFactory factory)
        {
            return _repositoryDocument.Update(model, factory);
        }
    }
}
