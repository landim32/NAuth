using NAuth.Domain.Factory.Interfaces;
using System.Collections.Generic;

namespace NAuth.Domain.Models.Models
{
    public interface IRoleModel
    {
        long RoleId { get; set; }
        string Slug { get; set; }
        string Name { get; set; }

        IRoleModel Insert(IRoleDomainFactory factory);
        IRoleModel Update(IRoleDomainFactory factory);
        IRoleModel GetById(long roleId, IRoleDomainFactory factory);
        IRoleModel GetBySlug(string slug, IRoleDomainFactory factory);
        IEnumerable<IRoleModel> ListRoles(IRoleDomainFactory factory);
        bool ExistSlug(long roleId, string slug);
        void Delete(long roleId);
    }
}
