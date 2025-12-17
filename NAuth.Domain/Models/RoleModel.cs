using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using System.Collections.Generic;

namespace NAuth.Domain.Models
{
    public class RoleModel : IRoleModel
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IRoleRepository<IRoleModel, IRoleDomainFactory> _repositoryRole;

        public RoleModel(IUnitOfWork unitOfWork, IRoleRepository<IRoleModel, IRoleDomainFactory> repositoryRole)
        {
            _unitOfWork = unitOfWork;
            _repositoryRole = repositoryRole;
        }

        public long RoleId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }

        public bool ExistSlug(long roleId, string slug)
        {
            return _repositoryRole.ExistSlug(roleId, slug);
        }

        public IRoleModel GetById(long roleId, IRoleDomainFactory factory)
        {
            return _repositoryRole.GetById(roleId, factory);
        }

        public IRoleModel GetBySlug(string slug, IRoleDomainFactory factory)
        {
            return _repositoryRole.GetBySlug(slug, factory);
        }

        public IRoleModel Insert(IRoleDomainFactory factory)
        {
            return _repositoryRole.Insert(this, factory);
        }

        public IEnumerable<IRoleModel> ListRoles(int take, IRoleDomainFactory factory)
        {
            return _repositoryRole.ListRoles(take, factory);
        }

        public IRoleModel Update(IRoleDomainFactory factory)
        {
            return _repositoryRole.Update(this, factory);
        }
    }
}
