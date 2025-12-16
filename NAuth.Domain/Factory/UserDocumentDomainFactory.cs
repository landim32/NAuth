using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class UserDocumentDomainFactory : IUserDocumentDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> _repositoryDocument;

        public UserDocumentDomainFactory(IUnitOfWork unitOfWork, IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> repositoryDocument)
        {
            _unitOfWork = unitOfWork;
            _repositoryDocument = repositoryDocument;
        }

        public IUserDocumentModel BuildUserDocumentModel()
        {
            return new UserDocumentModel(_unitOfWork, _repositoryDocument);
        }
    }
}
