using System.Collections.Generic;

namespace NAuth.Infra.Interfaces.Repository
{
    public interface IRoleRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        TModel GetById(long roleId, TFactory factory);
        TModel GetBySlug(string slug, TFactory factory);
        IEnumerable<TModel> ListRoles(int take, TFactory factory);
        bool ExistSlug(long roleId, string slug);
    }
}
