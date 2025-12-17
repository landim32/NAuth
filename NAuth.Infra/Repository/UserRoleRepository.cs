using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Infra.Repository
{
    public class UserRoleRepository : IUserRoleRepository<IRoleModel, IRoleDomainFactory>
    {
        protected readonly NAuthContext _context;

        public UserRoleRepository(NAuthContext context)
        {
            _context = context;
        }

        private IRoleModel DbToModel(IRoleDomainFactory factory, Role role)
        {
            var model = factory.BuildRoleModel();
            model.RoleId = role.RoleId;
            model.Slug = role.Slug;
            model.Name = role.Name;
            return model;
        }

        public IEnumerable<IRoleModel> ListRolesByUser(long userId, IRoleDomainFactory factory)
        {
            var rows = _context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Roles)
                .SelectMany(u => u.Roles)
                .OrderBy(r => r.Name)
                .ToList();
            
            return rows.Select(x => DbToModel(factory, x));
        }

        public void AddRoleToUser(long userId, long roleId)
        {
            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserId == userId);
            
            if (user == null)
                return;

            var role = _context.Roles.Find(roleId);
            if (role == null)
                return;

            if (!user.Roles.Any(r => r.RoleId == roleId))
            {
                user.Roles.Add(role);
                _context.SaveChanges();
            }
        }

        public void RemoveRoleFromUser(long userId, long roleId)
        {
            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserId == userId);
            
            if (user == null)
                return;

            var role = user.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role != null)
            {
                user.Roles.Remove(role);
                _context.SaveChanges();
            }
        }

        public void RemoveAllRolesFromUser(long userId)
        {
            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserId == userId);
            
            if (user == null)
                return;

            user.Roles.Clear();
            _context.SaveChanges();
        }

        public bool UserHasRole(long userId, long roleId)
        {
            return _context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Roles)
                .SelectMany(u => u.Roles)
                .Any(r => r.RoleId == roleId);
        }

        public bool UserHasRoleBySlug(long userId, string roleSlug)
        {
            return _context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Roles)
                .SelectMany(u => u.Roles)
                .Any(r => r.Slug == roleSlug);
        }
    }
}
