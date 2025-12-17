using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class UserDomainFactory : IUserDomainFactory
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserRepository<IUserModel, IUserDomainFactory> _repositoryUser;
        protected readonly IUserRoleRepository<IRoleModel, IRoleDomainFactory> _repositoryUserRole;

        public UserDomainFactory(
            IUnitOfWork unitOfWork, 
            IUserRepository<IUserModel, IUserDomainFactory> repositoryUser,
            IUserRoleRepository<IRoleModel, IRoleDomainFactory> repositoryUserRole
        )
        {
            _unitOfWork = unitOfWork;
            _repositoryUser = repositoryUser;
            _repositoryUserRole = repositoryUserRole;
        }

        public IUserModel BuildUserModel()
        {
            return new UserModel(_unitOfWork, _repositoryUser, _repositoryUserRole);
        }
    }
}
