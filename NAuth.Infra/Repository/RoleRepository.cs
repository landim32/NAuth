using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces.Repository;
using System.Collections.Generic;
using System.Linq;

namespace NAuth.Infra.Repository
{
    public class RoleRepository : IRoleRepository<IRoleModel, IRoleDomainFactory>
    {
        protected readonly NAuthContext _context;

        public RoleRepository(NAuthContext context)
        {
            _context = context;
        }

        private static IRoleModel DbToModel(IRoleDomainFactory factory, Role role)
        {
            var model = factory.BuildRoleModel();
            model.RoleId = role.RoleId;
            model.Slug = role.Slug;
            model.Name = role.Name;
            return model;
        }

        private static void ModelToDb(IRoleModel model, Role row)
        {
            row.RoleId = model.RoleId;
            row.Slug = model.Slug;
            row.Name = model.Name;
        }

        public bool ExistSlug(long roleId, string slug)
        {
            return _context.Roles.Where(x => x.Slug == slug && (roleId == 0 || x.RoleId != roleId)).Any();
        }

        public IRoleModel GetById(long roleId, IRoleDomainFactory factory)
        {
            var row = _context.Roles.Find(roleId);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }

        public IRoleModel GetBySlug(string slug, IRoleDomainFactory factory)
        {
            var row = _context.Roles.Where(x => x.Slug == slug).FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public IRoleModel Insert(IRoleModel model, IRoleDomainFactory factory)
        {
            var role = new Role();
            ModelToDb(model, role);
            _context.Add(role);
            _context.SaveChanges();
            model.RoleId = role.RoleId;
            return model;
        }

        public IEnumerable<IRoleModel> ListRoles(int take, IRoleDomainFactory factory)
        {
            var rows = _context.Roles.OrderBy(x => x.Name).Take(take).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IRoleModel Update(IRoleModel model, IRoleDomainFactory factory)
        {
            var row = _context.Roles.Where(x => x.RoleId == model.RoleId).FirstOrDefault();
            ModelToDb(model, row);
            _context.Roles.Update(row);
            _context.SaveChanges();
            return model;
        }
    }
}
