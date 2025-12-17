using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;

namespace NAuth.Domain.Factory
{
    public class RoleDomainFactory : IRoleDomainFactory
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRoleRepository<IRoleModel, IRoleDomainFactory> _repositoryRole;

        public RoleDomainFactory(IUnitOfWork unitOfWork, IRoleRepository<IRoleModel, IRoleDomainFactory> repositoryRole)
        {
            _unitOfWork = unitOfWork;
            _repositoryRole = repositoryRole;
        }

        public IRoleModel BuildRoleModel()
        {
            return new RoleModel(_unitOfWork, _repositoryRole);
        }
    }
}
