using NAuth.Domain.Models.Models;
using NAuth.DTO.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NAuth.Domain.Services.Interfaces
{
    public interface IRoleService
    {
        IRoleModel GetById(long roleId);
        IRoleModel GetBySlug(string slug);
        IEnumerable<IRoleModel> ListRoles();
        Task<IRoleModel> Insert(RoleInfo roleInfo);
        Task<IRoleModel> Update(RoleInfo roleInfo);
        void Delete(long roleId);
    }
}
