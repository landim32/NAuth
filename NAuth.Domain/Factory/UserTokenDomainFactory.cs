using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class UserTokenDomainFactory : IUserTokenDomainFactory
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory> _repositoryToken;

        public UserTokenDomainFactory(IUnitOfWork unitOfWork, IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory> repositoryToken)
        {
            _unitOfWork = unitOfWork;
            _repositoryToken = repositoryToken;
        }
        public IUserTokenModel BuildUserTokenModel()
        {
            return new UserTokenModel(_unitOfWork, _repositoryToken);
        }
    }
}
