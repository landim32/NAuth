using Core.Domain;
using Core.Domain.Repository;
using NAuth.Domain.Impl.Models;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAuth.Domain.Impl.Factory
{
    public class UserTokenDomainFactory : IUserTokenDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory> _repositoryToken;

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
