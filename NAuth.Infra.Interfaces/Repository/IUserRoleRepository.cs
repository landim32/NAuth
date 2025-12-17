using System.Collections.Generic;

namespace NAuth.Infra.Interfaces.Repository
{
    public interface IUserRoleRepository<TRoleModel, TRoleFactory>
    {
        IEnumerable<TRoleModel> ListRolesByUser(long userId, TRoleFactory factory);
        void AddRoleToUser(long userId, long roleId);
        void RemoveRoleFromUser(long userId, long roleId);
        void RemoveAllRolesFromUser(long userId);
        bool UserHasRole(long userId, long roleId);
        bool UserHasRoleBySlug(long userId, string roleSlug);
    }
}
